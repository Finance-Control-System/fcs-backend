using Application.Abstractions.Cryptography;
using Application.Abstractions.Providers;
using Application.Abstractions.Repositories;
using Domain.CQRS.Requests.Authentication;
using Domain.CQRS.Responses.Authentication;
using Domain.Exceptions;
using MediatR;

namespace Application.Handlers.Authentication;

internal class AuthenticateUserHandler(
        IUserRepository userRepository,
        IPasswordHashChecker passwordHashChecker,
        IJwtProvider jwtProvider,
        IRenewTokenRepository renewTokenRepository
    ) : IRequestHandler<AuthenticateUserRequest, AuthenticateUserResponse>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHashChecker _passwordHashChecker = passwordHashChecker;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IRenewTokenRepository _renewTokenRepository = renewTokenRepository;

    public async Task<AuthenticateUserResponse> Handle(AuthenticateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        BadRequestException.ThrowIfNull(user, "User with given email was not found.");

        var isPasswordMatch = _passwordHashChecker.HashesMath(user.HashedPassword!, request.Password, user.Salt!);
        if (!isPasswordMatch)
            throw new BadRequestException("Invalid password.");

        var authToken = _jwtProvider.Create(user);
        var renewToken = _jwtProvider.GenerateRandom();

        await _renewTokenRepository.AddOrUpdateAsync(user.Id, renewToken, cancellationToken);

        return new AuthenticateUserResponse(authToken, renewToken, user.Email!);
    }
}
