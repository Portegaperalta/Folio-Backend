using Folio.Core.Domain;
using Folio.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Folio.Infrastructure.Identity
{
    public sealed class TokenGenerator : ITokenGenerator
    {
        private readonly JwtOptions _jwtOptions;

        public TokenGenerator(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateJwt(User userEntity)
        {
            var claims = new List<Claim>
            {
                new Claim("id",userEntity.Id.ToString()),
                new Claim("email", userEntity.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
                signingCredentials: credentials
                );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }
    }
}