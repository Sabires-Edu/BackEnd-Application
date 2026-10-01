using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Sabires_Backend_Application.Utils
{
    public class TokenGenerator(IConfiguration config)
    {
        private readonly IConfiguration _config = config;

        public string GenerateJwt(Guid UserId, string Email, string Role)
        {
            var jwk = _config.GetSection("Supabase:PrivateJwk").Get<JwkOptions>();

            var parameters = new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,

                Q = new ECPoint
                {
                    X = Convert.FromBase64String(jwk.X),
                    Y = Convert.FromBase64String(jwk.Y)
                },

                D = Convert.FromBase64String(jwk.D)
            };

            using var ecdsa = ECDsa.Create(parameters);

            var securityKey = new ECDsaSecurityKey(ecdsa)
            {
                KeyId = jwk.Kid
            };

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, UserId.ToString()),
                new(JwtRegisteredClaimNames.Email, Email),
                new("role", "authenticated"),
                new("user_role", Role)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }

    public class SupabaseJwtOptions
    {
        public JwkOptions PrvivateJwk { get; set; } = new();
    }

    public class JwkOptions
    {
        public string Kty { get; set; } = string.Empty;
        public string Kid { get; set; } = string.Empty;
        public string Crv { get; set; } = string.Empty;
        public string X { get; set; } = string.Empty;
        public string Y { get; set; } = string.Empty;
        public string D { get; set; } = string.Empty;
    }
}
