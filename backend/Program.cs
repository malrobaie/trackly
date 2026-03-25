using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Trackly.Api.Enums;
using Trackly.Api.Interfaces;
using Trackly.Api.Services;
using Trackly.Api.DTOs;
using Trackly.Api.Models;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "Frontend";

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var firstError = context.ModelState.Values
            .SelectMany(entry => entry.Errors)
            .Select(error => error.ErrorMessage)
            .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message))
            ?? "The request is invalid.";

        return new BadRequestObjectResult(new ErrorResponseDto
        {
            Message = firstError
        });
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.MapType<Carrier>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum
            .GetNames<Carrier>()
            .Select(name => new OpenApiString(name))
            .Cast<IOpenApiAny>()
            .ToList()
    });
});
builder.Services.AddHttpClient();
builder.Services.Configure<UspsApiOptions>(builder.Configuration.GetSection(UspsApiOptions.SectionName));
builder.Services.AddSingleton<ITrackingProvider, UspsTrackingProvider>();
builder.Services.AddSingleton<ITrackingProvider, UpsTrackingProvider>();
builder.Services.AddSingleton<ITrackingProvider, FedExTrackingProvider>();
builder.Services.AddSingleton<ITrackingProvider, DhlTrackingProvider>();
builder.Services.AddSingleton<ITrackingProvider, OnTracTrackingProvider>();
builder.Services.AddSingleton<ITrackingOrchestratorService, TrackingOrchestratorService>();
builder.Services.AddSingleton<ITrackingService, InMemoryTrackingService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicyName);
app.MapControllers();

app.Run();
