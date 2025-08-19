using AutoMapper;
using BCrypt.Net;
using MediatR;
using Task2.Application.Commands;
using Task2.Application.DTOs;
using Task2.Core.Entities;
using Task2.Core.Interfaces;

namespace Task2.Application.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserResponseDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(IRepository<User> userRepository, IJwtService jwtService, IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = (await _userRepository.GetAsync(u => u.Email == request.UserRegisterDto.Email)).FirstOrDefault();
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var user = new User
            {
                Email = request.UserRegisterDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.UserRegisterDto.Password),
                FirstName = request.UserRegisterDto.FirstName,
                LastName = request.UserRegisterDto.LastName,
                PhoneNumber = request.UserRegisterDto.PhoneNumber
            };

            var createdUser = await _userRepository.AddAsync(user);
            var token = _jwtService.GenerateToken(createdUser);

            var response = _mapper.Map<UserResponseDto>(createdUser);
            response.Token = token;

            return response;
        }
    }

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, UserResponseDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public LoginUserCommandHandler(IRepository<User> userRepository, IJwtService jwtService, IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = (await _userRepository.GetAsync(u => u.Email == request.UserLoginDto.Email)).FirstOrDefault();
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.UserLoginDto.Password, user.PasswordHash))
            {
                throw new InvalidOperationException("Invalid email or password.");
            }

            var token = _jwtService.GenerateToken(user);

            var response = _mapper.Map<UserResponseDto>(user);
            response.Token = token;

            return response;
        }
    }
} 