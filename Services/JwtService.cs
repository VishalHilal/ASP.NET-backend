using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AIImageGeneratorBackend.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(string username)
        {
            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            if (string.IsNullOrEmpty(key) || key.Length < 16)
                throw new InvalidOperationException("JWT Key must be at least 16 characters long. Configure it using: dotnet user-secrets set \"Jwt:Key\" \"YourSecretKeyHere123456789\"");

            if (string.IsNullOrEmpty(issuer))
                issuer = "AIImageGeneratorBackend";

            if (string.IsNullOrEmpty(audience))
                audience = "AIImageGeneratorUsers";

            var keyBytes = Encoding.UTF8.GetBytes(key);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
