using MediatR;
using Microsoft.AspNetCore.Mvc;
using Task2.Application.Commands;
using Task2.Application.DTOs;

namespace Task2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            try
            {
                var command = new RegisterUserCommand { UserRegisterDto = userRegisterDto };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new { message = "Invalid operation." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while registering the user." });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login([FromBody] UserLoginDto userLoginDto)
        {
            try
            {
                var command = new LoginUserCommand { UserLoginDto = userLoginDto };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new { message = "Invalid operation." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while logging in." });
            }
        }
    }
} 