using Application.Abstractions.Cryptography;
using Application.Abstractions.Providers;
using Application.Abstractions.Repositories;
using Domain.CQRS.Requests.Authentication;
using Domain.CQRS.Responses.Authentication;
using Domain.Entities.Account;
using Domain.Exceptions;
using MediatR;

namespace Application.Handlers.Authentication;

internal class RegisterUserHandler(
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IJwtProvider jwtProvider,
    IRenewTokenRepository renewTokenRepository)
    : IRequestHandler<RegisterUserRequest, AuthenticateUserResponse>
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IRenewTokenRepository _renewTokenRepository = renewTokenRepository;

    public async Task<AuthenticateUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        if (!await _userRepository.IsEmailUniqueAsync(request.Email, cancellationToken))
            throw new BadRequestException("User with current email is already registered.");

        var salt = _passwordHasher.GenerateSalt();

        var user = new User
        {
            Email = request.Email,
            Salt = salt,
            HashedPassword = _passwordHasher.HashPassword(request.Password, salt)
        };

        await _userRepository.AddUserAsync(user, cancellationToken);

        // Auth actions
        var authToken = _jwtProvider.Create(user);
        var renewToken = _jwtProvider.GenerateRandom();

        await _renewTokenRepository.AddOrUpdateAsync(user.Id, renewToken, cancellationToken);

        return new AuthenticateUserResponse(authToken, renewToken, user.Email!);
    }
}
