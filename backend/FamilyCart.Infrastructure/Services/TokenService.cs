namespace FamilyCart.Infrastructure.Services
{
    using FamilyCart.Core.Interfaces;
    using FamilyCart.Core.Models;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Configuration;
    using System.Text;
    using FamilyCart.Core.DTOs;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        
        public TokenService (IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public TokenResponseDto GetToken(User user)
        {
            string secret = _configuration["Authentication:Schemes:Bearer:SigningKeys:0:Value"] ?? throw new InvalidOperationException("JWT signing key not found.");
            string issuer = _configuration["Authentication:Schemes:Bearer:ValidIssuer"] ?? throw new InvalidOperationException("JWT issuer not found.");
            string audience = _configuration["Authentication:Schemes:Bearer:ValidAudiences:0"] ?? throw new InvalidOperationException("JWT audience not found.");

            Claim[] claims = [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? "")
            ];

            var key = new SymmetricSecurityKey(Convert.FromBase64String(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                signingCredentials: credentials,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30)
            );

            var token = new TokenResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                Expiration = jwt.ValidTo
            };

            return token;
        }
    }
}