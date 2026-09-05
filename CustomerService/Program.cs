using CustomerService.Application.Common.Authorization;
using CustomerService.Application.Common.Interfaces;
using CustomerService.Application.Features.Customers.Mappings;
using CustomerService.Domain.Constants;
using CustomerService.Infrastructure.DependencyInjection;
using CustomerService.Infrastructure.Logging;
using CustomerService.Infrastructure.Messaging;
using CustomerService.Infrastructure.Middleware;
using CustomerService.Infrastructure.Persistence.Repositories;
using CustomerService.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.ParameterLocation.Header,
            Description =
                "Enter your JWT token. Example: Bearer eyJhbGciOiJIUzI1Ni..."
        });

    options.AddSecurityRequirement(document =>
        new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [
                new Microsoft.OpenApi.OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = []
        });
});

builder.Services.AddInfrastructure(builder.Configuration);
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
builder.Services.AddAutoMapper(typeof(CustomerProfile));
// JWT Authentication

var jwtKey =
    builder.Configuration["Jwt:Key"];

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"];

var jwtAudience =
    builder.Configuration["Jwt:Audience"];


builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey!)),

                ValidateIssuer = true,

                ValidIssuer = jwtIssuer,

                ValidateAudience = true,

                ValidAudience = jwtAudience,

                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AuthorizationPolicies.ViewCustomers,
        policy =>
        {
            policy.RequireRole(
                Roles.Admin,
                Roles.Manager,
                Roles.Officer);
        });

    options.AddPolicy(
        AuthorizationPolicies.ManageCustomers,
        policy =>
        {
            policy.RequireRole(
                Roles.Admin,
                Roles.Manager,
                Roles.Officer);
        });

    options.AddPolicy(
        AuthorizationPolicies.DeleteCustomers,
        policy =>
        {
            policy.RequireRole(
                Roles.Admin);
        });

    options.AddPolicy(
        AuthorizationPolicies.ChangeCustomerStatus,
        policy =>
        {
            policy.RequireRole(
                Roles.Admin,
                Roles.Manager);
        });
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.UseCors("AllowReact");
app.UseCorrelationId();

app.UseRequestLogging();
app.UseGlobalExceptionMiddleware();
//app.UseCustomMiddleware();
app.UseStaticFiles();
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

app.Run();