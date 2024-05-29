using Application.Abstractions.Providers;
using Domain.Configuration;
using Domain.Entities.Account;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Providers;

internal sealed class JwtProvider : IJwtProvider
{
    private readonly AppSettings _appSettings;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public int ExpirationInMinutes { get; } = 120;

    public JwtProvider(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public string Create(User user)
    {
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                    new Claim("id", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!)
                ]),
            Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(ExpirationInMinutes)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    public string GetEmailFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return string.Empty;

        var jwtToken = _tokenHandler.ReadJwtToken(token);
        var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "email");

        return emailClaim == null ? string.Empty : emailClaim.Value;
    }

    public string GenerateRandom(int size = 32)
    {
        var randomNumber = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
