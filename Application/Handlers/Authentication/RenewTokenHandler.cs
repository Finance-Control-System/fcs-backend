using Application.Abstractions.Providers;
using Application.Abstractions.Repositories;
using Domain.CQRS.Requests.Authentication;
using Domain.CQRS.Responses.Authentication;
using Domain.Exceptions;
using MediatR;

namespace Application.Handlers.Authentication;

internal class RenewTokenHandler(
        IJwtProvider jwtProvider,
        IUserRepository userRepository,
        IRenewTokenRepository renewTokenRepository
    ) : IRequestHandler<RenewTokenRequest, AuthenticateUserResponse>
{
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRenewTokenRepository _renewTokenRepository = renewTokenRepository;

    public async Task<AuthenticateUserResponse> Handle(RenewTokenRequest request, CancellationToken cancellationToken)
    {
        BadRequestException.ThrowIfEmpty(request.AuthToken, "Missing AuthToken.");

        var email = _jwtProvider.GetEmailFromToken(request.AuthToken);

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        BadRequestException.ThrowIfNull(user, "User not found.");

        var foundId = await _renewTokenRepository.FindRenewTokenIdByUserIdAndTokenAsync(user.Id, request.RenewToken, cancellationToken);
        BadRequestException.ThrowIfZero(foundId, "Failed found renew token.");

        var authToken = _jwtProvider.Create(user);
        var newRenewToken = _jwtProvider.GenerateRandom();
        await _renewTokenRepository.UpdateRenewTokenAsync(foundId, newRenewToken, cancellationToken);

        return new AuthenticateUserResponse(authToken, newRenewToken, user.Email!);
    }
}
