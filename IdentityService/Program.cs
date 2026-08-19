
using IdentityService.Data;
using IdentityService.Extensions;
using IdentityService.Features.Auth.Interfaces;
using IdentityService.Features.Auth.Repositories;
using IdentityService.Features.Auth.Services;

var builder = WebApplication.CreateBuilder(args);

//// Register Services
//builder.Services.AddSingleton<DapperContext>();
//builder.Services.AddScoped<IAuthRepository, AuthRepository>();
//builder.Services.AddScoped<IAuthService, AuthService>();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddApplicationServices();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddSwaggerDocumentation();


builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowReact",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.UseCors("AllowReact");
app.MapControllers();

app.Run();