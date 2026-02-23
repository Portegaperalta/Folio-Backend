using Folio.Core.Domain.Entities;
using Folio.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Folio.Infrastructure.Identity
{
    public sealed class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;

        public TokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwt(User userEntity)
        {
            var jwtOptions = _configuration.GetSection("JwtOptions");
            var secretKey = jwtOptions.GetValue<string>("SecretKey");

            var claims = new List<Claim>
            {
                new Claim("id",userEntity.Id.ToString()),
                new Claim("email", userEntity.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                issuer: jwtOptions.GetValue<string>("Issuer"),
                audience: jwtOptions.GetValue<string>("Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(jwtOptions.GetValue<double>("ExpiryHours")),
                signingCredentials: credentials
                );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }
    }
}