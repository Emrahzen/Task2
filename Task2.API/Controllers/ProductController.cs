using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task2.Application.Commands;
using Task2.Application.DTOs;
using Task2.Application.Queries;

namespace Task2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
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
        public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductDto createProductDto)
        {
            try
            {
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
        public async Task<ActionResult<ProductResponseDto>> Update(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
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
                var command = new DeleteProductCommand { Id = id };
                var result = await _mediator.Send(command);
                
                if (!result)
                    return NotFound(new { message = "Product not found." });

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the product." });
            }
        }
    }
} 