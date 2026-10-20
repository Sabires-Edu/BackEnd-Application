using Microsoft.IdentityModel.Tokens;
using Sabires_Backend_Application.Utils.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace Sabires_Backend_Application.Utils.Token
{
    public class TokenGenerator
    {
        
        public readonly JwkOptions jwk;
        private readonly IConfiguration _Configuration;

        public TokenGenerator(IConfiguration Configuration)
        {
            _Configuration = Configuration;

            var jwkJson = Configuration["Supabase:PrivateJwk"];

            if (string.IsNullOrEmpty(jwkJson))
                throw new InternalServerException("Supabase Private JWK não configurada.");

            jwk = JsonSerializer.Deserialize<JwkOptions>(jwkJson) ?? throw new InternalServerException("Não foi possível obter a chave JWK privada.");
        }

        public string GenerateJwt(Guid UserId, string Email, string Role)
        {
            using ECDsa ecdsa = CreateECDsa();

            var securityKey = new ECDsaSecurityKey(ecdsa)
            {
                KeyId = jwk.Kid,

                CryptoProviderFactory = new CryptoProviderFactory
                {
                    CacheSignatureProviders = false
                }
            };

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256);

            if(string.IsNullOrEmpty(_Configuration["Jwt:Issuer"]))
                throw new InternalServerException("JWT Issuer não configurado.");

            if(string.IsNullOrEmpty(_Configuration["Jwt:Audience"]))
                throw new InternalServerException("JWT Audience não configurado.");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, UserId.ToString()),
                new(JwtRegisteredClaimNames.Email, Email),
                new("role", "authenticated"),
                new("user_role", Role),
                new(JwtRegisteredClaimNames.Iss, _Configuration["Jwt:Issuer"]),
                new(JwtRegisteredClaimNames.Aud, _Configuration["Jwt:Audience"])
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            var handler = new JwtSecurityTokenHandler();

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    
        public ECDsa CreateECDsa()
        {
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

            return ECDsa.Create(parameters);
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
