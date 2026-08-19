using CustomerService.Application.Features.Customers.Mappings;
using CustomerService.Infrastructure.DependencyInjection;
using CustomerService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

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
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseCorrelationId();

//app.UseRequestLogging();

app.UseCustomMiddleware();

app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

app.Run();