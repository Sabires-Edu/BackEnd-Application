using Sabires_Backend_Application.Repositories;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Utils;
using Sabires_Backend_Application.Utils.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<SupabaseClienteService>();
builder.Services.AddScoped<TokenGenerator>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddExceptionHandler<CustomerExceptionsHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();

//app.Run("http://0.0.0.0:5083");
app.Run();
