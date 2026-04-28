using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Watcher.Api.Validators;
using Watcher.Domain.Entities;
using Watcher.Domain.Interfaces;
using Watcher.Domain.Services;
using Watcher.Infrastructure.Mocks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IClusterConfigurationRepository, ClusterConfigurationRepository>();
builder.Services.AddScoped<IClusterConfigurationService, ClusterConfigurationService>();
builder.Services.AddSingleton<IJobCategoryRepository, JobCategoryRepository>();
builder.Services.AddScoped<IJobCategoryService, JobCategoryService>();
builder.Services.AddScoped<IClassificationService, ClassificationService>();
builder.Services.AddScoped<IMonthlyIncomeMappingService, MonthlyIncomeMappingService>();
builder.Services.AddSingleton<IMonthlyIncomeMappingRepository, MonthlyIncomeMappingRepository>();
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IPenaltyRuleService, PenaltyRuleService>();
builder.Services.AddSingleton<IPenaltyRuleRepository, PenaltyRuleRepository>();
builder.Services.AddSingleton<Watcher.Api.Services.RequestMetrics>();

builder.Services.AddMediatR(typeof(Watcher.Domain.Commands.Request.ClassifyCustomerCommand).Assembly);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Watcher.Domain.Handlers.ClassifyCustomerCommandHandler).Assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.Use(async (context, next) =>
{
    var metrics = context.RequestServices.GetRequiredService<Watcher.Api.Services.RequestMetrics>();
    metrics.TotalRequests++;
    await next();
});

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    .WithName("HealthCheck")
    .WithOpenApi();

app.MapGet("/metrics", (Watcher.Api.Services.RequestMetrics metrics) => Results.Ok(metrics))
    .WithName("Metrics")
    .WithOpenApi();

app.Run();

public partial class Program { }
