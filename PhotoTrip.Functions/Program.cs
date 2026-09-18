using Azure.Monitor.OpenTelemetry.Exporter;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using PhotoTrip.BLL.Services;
using PhotoTrip.BLL.Services.Interfaces;
using PhotoTrip.DAL.Data;
using PhotoTrip.DAL.Repositories;
using PhotoTrip.DAL.Repositories.Interfaces;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddDbContext<PhotoDbContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IPlaceRepository, PlaceRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddAutoMapper(cfg => { }, typeof(IPlaceService).Assembly);

builder.Services.AddSingleton(_ =>
    new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();