using MediatR;
using Task2.Application.DTOs;

namespace Task2.Application.Commands
{
    public class RegisterUserCommand : IRequest<UserResponseDto>
    {
        public UserRegisterDto UserRegisterDto { get; set; } = new();
    }

    public class LoginUserCommand : IRequest<UserResponseDto>
    {
        public UserLoginDto UserLoginDto { get; set; } = new();
    }
} 