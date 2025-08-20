using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using Task2.Application.Commands;
using Task2.Application.DTOs;
using Task2.Application.Queries;
using Task2.API.Models;

namespace Task2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IMediator mediator, IWebHostEnvironment environment, ILogger<ProductController> logger)
        {
            _mediator = mediator;
            _environment = environment;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll([FromQuery] ProductFilterDto? filter)
        {
            try
            {
                var query = new GetAllProductsQuery { Filter = filter };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products." });
            }
        }

        

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetById(int id)
        {
            try
            {
                var query = new GetProductByIdQuery { Id = id };
                var result = await _mediator.Send(query);
                
                if (result == null)
                    return NotFound(new { message = "Product not found." });

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the product." });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetByCategory(string category)
        {
            try
            {
                var query = new GetProductsByCategoryQuery { Category = category };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products by category." });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> Search([FromQuery] string searchTerm)
        {
            try
            {
                var query = new SearchProductsQuery { SearchTerm = searchTerm };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while searching products." });
            }
        }

        [HttpPost]
        [Authorize]
        [RequestSizeLimit(10_000_000)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductResponseDto>> Create([FromForm] CreateProductForm form)
        {
            try
            {
                var createProductDto = new CreateProductDto
                {
                    Name = form.Name,
                    Description = form.Description,
                    Price = form.Price,
                    StockQuantity = form.StockQuantity,
                    Category = form.Category,
                    Brand = form.Brand
                };

                if (form.File != null && form.File.Length > 0)
                {
                    if (!form.File.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    {
                        return BadRequest(new { message = "Sadece resim dosyaları yüklenebilir." });
                    }

                    var imagesRoot = Path.Combine(_environment.ContentRootPath, "Image");
                    if (!Directory.Exists(imagesRoot))
                    {
                        Directory.CreateDirectory(imagesRoot);
                    }

                    var guid = Guid.NewGuid().ToString("N");
                    var extension = Path.GetExtension(form.File.FileName);
                    var safeExtension = string.IsNullOrWhiteSpace(extension) ? ".jpg" : extension;
                    var fileName = guid + safeExtension;
                    var fullPath = Path.Combine(imagesRoot, fileName);

                    await using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await form.File.CopyToAsync(stream);
                    }

                    // Store GUID in DB
                    createProductDto.ImageUrl = guid;
                }
                var command = new CreateProductCommand { CreateProductDto = createProductDto };
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "An error occurred while creating the product.",
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [RequestSizeLimit(10_000_000)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductResponseDto>> Update(int id, [FromForm] UpdateProductForm form)
        {
            try
            {
                var updateProductDto = new UpdateProductDto
                {
                    Name = form.Name,
                    Description = form.Description,
                    Price = form.Price,
                    StockQuantity = form.StockQuantity,
                    Category = form.Category,
                    Brand = form.Brand,
                    IsActive = form.IsActive
                };

                if (form.File != null && form.File.Length > 0)
                {
                    if (!form.File.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    {
                        return BadRequest(new { message = "Sadece resim dosyaları yüklenebilir." });
                    }

                    var imagesRoot = Path.Combine(_environment.ContentRootPath, "Image");
                    if (!Directory.Exists(imagesRoot))
                    {
                        Directory.CreateDirectory(imagesRoot);
                    }

                    var guid = Guid.NewGuid().ToString("N");
                    var extension = Path.GetExtension(form.File.FileName);
                    var safeExtension = string.IsNullOrWhiteSpace(extension) ? ".jpg" : extension;
                    var fileName = guid + safeExtension;
                    var fullPath = Path.Combine(imagesRoot, fileName);

                    await using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await form.File.CopyToAsync(stream);
                    }

                    updateProductDto.ImageUrl = guid;
                }
                var command = new UpdateProductCommand { Id = id, UpdateProductDto = updateProductDto };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                return NotFound(new { message = "Product not found." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating the product." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Get product to know its image GUID (if any)
                var existing = await _mediator.Send(new GetProductByIdQuery { Id = id });
                if (existing == null)
                {
                    return NotFound(new { message = "Product not found." });
                }

                var command = new DeleteProductCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new { message = "Product not found." });
                }

                // After successful delete, remove image file if present
                if (!string.IsNullOrWhiteSpace(existing.ImageUrl))
                {
                    try
                    {
                        var imagesRoot = Path.Combine(_environment.ContentRootPath, "Image");
                        if (!Directory.Exists(imagesRoot))
                        {
                            Directory.CreateDirectory(imagesRoot);
                        }

                        var imageRef = existing.ImageUrl.Trim();
                        _logger.LogInformation("Attempting to delete image for product {ProductId} with reference {ImageRef}", id, imageRef);

                        // If imageRef is a full URL, extract last segment
                        if (imageRef.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                var uri = new Uri(imageRef);
                                imageRef = uri.Segments.Last().Trim('/'); // could be 'image/{guid}' or '{fileName}'
                                if (imageRef.Equals("image", StringComparison.OrdinalIgnoreCase) && uri.Segments.Length >= 3)
                                {
                                    imageRef = uri.Segments[^1].Trim('/');
                                }
                                _logger.LogInformation("Extracted image reference from URL: {ImageRef}", imageRef);
                            }
                            catch { }
                        }

                        // If it looks like a filename with extension, delete exact; else treat as GUID pattern
                        var candidateFiles = new List<string>();
                        if (imageRef.Contains('.'))
                        {
                            candidateFiles.Add(Path.Combine(imagesRoot, imageRef));
                        }
                        else
                        {
                            candidateFiles.AddRange(Directory.GetFiles(imagesRoot, imageRef + ".*"));
                        }

                        _logger.LogInformation("Found {Count} candidate files to delete", candidateFiles.Count);
                        foreach (var filePath in candidateFiles.Distinct())
                        {
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                                _logger.LogInformation("Deleted image file {Path}", filePath);
                            }
                            else
                            {
                                _logger.LogWarning("Image file not found for deletion: {Path}", filePath);
                            }
                        }
                    }
                    catch
                    {
                        // ignore file delete errors
                    }
                }

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the product." });
            }
        }
    }
} 