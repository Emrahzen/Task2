using AutoMapper;
using MediatR;
using Task2.Application.Commands;
using Task2.Application.DTOs;
using Task2.Application.Queries;
using Task2.Core.Entities;
using Task2.Core.Interfaces;

namespace Task2.Application.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponseDto>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IRepository<Product> productRepository, ICacheService cacheService, IMapper mapper)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<ProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request.CreateProductDto);
            var createdProduct = await _productRepository.AddAsync(product);

            // Invalidate cache
            await _cacheService.RemoveByPatternAsync("products:*");

            return _mapper.Map<ProductResponseDto>(createdProduct);
        }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponseDto>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(IRepository<Product> productRepository, ICacheService cacheService, IMapper mapper)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<ProductResponseDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.Id);
            if (existingProduct == null)
            {
                throw new InvalidOperationException("Product not found.");
            }

            _mapper.Map(request.UpdateProductDto, existingProduct);
            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);

            // Invalidate cache
            await _cacheService.RemoveByPatternAsync("products:*");
            await _cacheService.RemoveAsync($"product:{request.Id}");

            return _mapper.Map<ProductResponseDto>(updatedProduct);
        }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ICacheService _cacheService;

        public DeleteProductCommandHandler(IRepository<Product> productRepository, ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var result = await _productRepository.DeleteAsync(request.Id);
            
            if (result)
            {
                // Invalidate cache
                await _cacheService.RemoveByPatternAsync("products:*");
                await _cacheService.RemoveAsync($"product:{request.Id}");
            }

            return result;
        }
    }

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductResponseDto?>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IRepository<Product> productRepository, ICacheService cacheService, IMapper mapper)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<ProductResponseDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // Try to get from cache first
            var cachedProduct = await _cacheService.GetAsync<ProductResponseDto>($"product:{request.Id}");
            if (cachedProduct != null)
            {
                return cachedProduct;
            }

            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return null;
            }

            var productDto = _mapper.Map<ProductResponseDto>(product);
            
            // Cache the result
            await _cacheService.SetAsync($"product:{request.Id}", productDto, TimeSpan.FromMinutes(30));

            return productDto;
        }
    }

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductResponseDto>>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(IRepository<Product> productRepository, ICacheService cacheService, IMapper mapper)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "products:all";
            if (request.Filter != null)
            {
                cacheKey = $"products:filtered:{request.Filter.SearchTerm}:{request.Filter.Category}:{request.Filter.Brand}:{request.Filter.MinPrice}:{request.Filter.MaxPrice}:{request.Filter.SortBy}:{request.Filter.Page}:{request.Filter.PageSize}";
            }

            // Try to get from cache first
            var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductResponseDto>>(cacheKey);
            if (cachedProducts != null)
            {
                return cachedProducts;
            }

            var products = await _productRepository.GetAllAsync();
            var filteredProducts = products.Where(p => p.IsActive);

            // Apply filters
            if (request.Filter != null)
            {
                if (!string.IsNullOrEmpty(request.Filter.SearchTerm))
                {
                    filteredProducts = filteredProducts.Where(p => 
                        p.Name.Contains(request.Filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        p.Description?.Contains(request.Filter.SearchTerm, StringComparison.OrdinalIgnoreCase) == true);
                }

                if (!string.IsNullOrEmpty(request.Filter.Category))
                {
                    filteredProducts = filteredProducts.Where(p => p.Category == request.Filter.Category);
                }

                if (!string.IsNullOrEmpty(request.Filter.Brand))
                {
                    filteredProducts = filteredProducts.Where(p => p.Brand == request.Filter.Brand);
                }

                if (request.Filter.MinPrice.HasValue)
                {
                    filteredProducts = filteredProducts.Where(p => p.Price >= request.Filter.MinPrice.Value);
                }

                if (request.Filter.MaxPrice.HasValue)
                {
                    filteredProducts = filteredProducts.Where(p => p.Price <= request.Filter.MaxPrice.Value);
                }

                if (request.Filter.IsActive.HasValue)
                {
                    filteredProducts = filteredProducts.Where(p => p.IsActive == request.Filter.IsActive.Value);
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(request.Filter.SortBy))
                {
                    filteredProducts = request.Filter.SortBy switch
                    {
                        "price_asc" => filteredProducts.OrderBy(p => p.Price),
                        "price_desc" => filteredProducts.OrderByDescending(p => p.Price),
                        "name_asc" => filteredProducts.OrderBy(p => p.Name),
                        "name_desc" => filteredProducts.OrderByDescending(p => p.Name),
                        _ => filteredProducts
                    };
                }

                // Apply pagination
                filteredProducts = filteredProducts
                    .Skip((request.Filter.Page - 1) * request.Filter.PageSize)
                    .Take(request.Filter.PageSize);
            }

            var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(filteredProducts);
            
            // Cache the result
            await _cacheService.SetAsync(cacheKey, productDtos, TimeSpan.FromMinutes(15));

            return productDtos;
        }
    }

    public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<ProductResponseDto>>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public GetProductsByCategoryQueryHandler(IRepository<Product> productRepository, ICacheService cacheService, IMapper mapper)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"products:category:{request.Category}";

            // Try to get from cache first
            var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductResponseDto>>(cacheKey);
            if (cachedProducts != null)
            {
                return cachedProducts;
            }

            var products = await _productRepository.GetAsync(p => p.Category == request.Category && p.IsActive);
            var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(products);
            
            // Cache the result
            await _cacheService.SetAsync(cacheKey, productDtos, TimeSpan.FromMinutes(15));

            return productDtos;
        }
    }

    public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, IEnumerable<ProductResponseDto>>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public SearchProductsQueryHandler(IRepository<Product> productRepository, ICacheService cacheService, IMapper mapper)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"products:search:{request.SearchTerm}";

            // Try to get from cache first
            var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductResponseDto>>(cacheKey);
            if (cachedProducts != null)
            {
                return cachedProducts;
            }

            var allProducts = await _productRepository.GetAllAsync();
            var searchResults = allProducts.Where(p => 
                p.IsActive && 
                (p.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                 p.Description?.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                 p.Brand?.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                 p.Category?.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) == true));

            var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(searchResults);
            
            // Cache the result
            await _cacheService.SetAsync(cacheKey, productDtos, TimeSpan.FromMinutes(10));

            return productDtos;
        }
    }
} 