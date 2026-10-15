using Microsoft.IdentityModel.Tokens;
using Sabires_Backend_Application.Utils.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace Sabires_Backend_Application.Utils
{
    public class TokenGenerator(IConfiguration Configuration)
    {
        public string GenerateJwt(Guid UserId, string Email, string Role)
        {
            var jwkJson = Configuration["Supabase:PrivateJwk"];

            if (string.IsNullOrEmpty(jwkJson))
                throw new InternalServerException("Supabase Private JWK não configurada.");

            var jwk = JsonSerializer.Deserialize<JwkOptions>(jwkJson) ?? throw new InternalServerException("Não foi possível obter a chave JWK privada.");

            var parameters = new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,

                Q = new ECPoint
                {
                    X = WebEncoders.Base64UrlDecode(jwk.X),
                    Y = WebEncoders.Base64UrlDecode(jwk.Y)
                },

                D = WebEncoders.Base64UrlDecode(jwk.D)
            };

            using var ecdsa = ECDsa.Create(parameters);

            var securityKey = new ECDsaSecurityKey(ecdsa)
            {
                KeyId = jwk.Kid,

                CryptoProviderFactory = new CryptoProviderFactory
                {
                    CacheSignatureProviders = false
                }
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

            var handler = new JwtSecurityTokenHandler();

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class JwkOptions
    {
        [JsonPropertyName("kty")]
        public string Kty { get; set; } = string.Empty;

        [JsonPropertyName("kid")]
        public string Kid { get; set; } = string.Empty;

        [JsonPropertyName("crv")]
        public string Crv { get; set; } = string.Empty;

        [JsonPropertyName("x")]
        public string X { get; set; } = string.Empty;

        [JsonPropertyName("y")]
        public string Y { get; set; } = string.Empty;

        [JsonPropertyName("d")]
        public string D { get; set; } = string.Empty;
    }
}
