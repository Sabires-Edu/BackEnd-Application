using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Sabires_Backend_Application.Repositories;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Utils.Exceptions;
using Sabires_Backend_Application.Utils.Token;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<SupabaseClientService>();
builder.Services.AddScoped<TokenGenerator>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddExceptionHandler<CustomerExceptionsHandler>();
builder.Services.AddProblemDetails();

TokenGenerator token = new TokenGenerator(builder.Configuration);
using ECDsa ecdsa = token.CreateECDsa();

var securityKey = new ECDsaSecurityKey(ecdsa)
{
    KeyId = token.jwk.Kid,

    CryptoProviderFactory = new CryptoProviderFactory
    {
        CacheSignatureProviders = false
    }
};

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,

            ValidAlgorithms = new[]
            {
                SecurityAlgorithms.EcdsaSha256
            },

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"]
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                string message = context.AuthenticateFailure switch
                {
                    SecurityTokenExpiredException => "O token de acesso expirou. Por favor realize o login novamente.",
                    SecurityTokenInvalidSignatureException => "A assinatura do token não é valida. Por favor realize o login novamente.",
                    SecurityTokenException => "O token de acesso é inválido. Por favor realize o login novamente.",
                    _ => "O token de acesso é inválido. Por favor realize o login novamente"
                };

                var response = new
                {
                    success = false,
                    title = "Houve um erro ao realizar a requisição",
                    detail = message,
                    status = 401,
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins(
                "http://127.0.0.1:5173",
                "http://0.0.0.0:5173",
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//app.Run("http://0.0.0.0:5083");
app.Run();
