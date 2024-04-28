using System.Text.Json.Serialization;
using Azure.Identity;
using ChaosMonkey.API;
using ChaosMonkey.API.Integrations;
using ChaosMonkey.API.Repositories;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Configuration.AddEnvironmentVariables(prefix: "CHAOS_MONKEY_");
builder.Services.AddControllers();
builder.Services.AddSingleton<AzureResourceManagerClient>(services =>
{
    var logger = services.GetService<ILoggerFactory>()?.CreateLogger<Program>() ?? new NullLogger<Program>();
    var configuration = services.GetService<IConfiguration>();
    if (configuration == null)
    {
        throw new Exception("No configuration was found");
    }

    var authMode = configuration.GetValue<string>("AUTH_MODE");
    if (authMode == "ClientSecret")
    {
        var tenantId = configuration.GetValue<string>("TENANT_ID");
        var appId = configuration.GetValue<string>("APP_ID");
        logger.LogInformation("Using client secret authentication for {tenantId} with {appId}", tenantId, appId);
        return new AzureResourceManagerClient(tenantId, appId, configuration.GetValue<string>("APP_SECRET"));
    }
    else
    {
        logger.LogInformation("Using default authentication given auth mode {authMode} is not supported", authMode);
        return new AzureResourceManagerClient();
    }
});
builder.Services.AddTransient<ApiManagementRepository>();
builder.Services.AddHostedService<ShutdownGatewayWorker>();
// Check https://learn.microsoft.com/en-gb/azure/azure-monitor/app/ilogger?tabs=dotnet6 
//https://learn.microsoft.com/en-gb/azure/azure-monitor/app/asp-net-core?tabs=netcorenew
// https://github.com/microsoft/ApplicationInsights-dotnet/blob/main/docs/concepts.md
// https://www.nuget.org/packages/Microsoft.ApplicationInsights/2.22.0
builder.Services.Configure<TelemetryConfiguration>(config =>
{
       var credential = new DefaultAzureCredential();
       config.SetAzureTokenCredential(credential);
});
builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
{
    ConnectionString = "InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint=https://xxxx.applicationinsights.azure.com/"
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseExceptionHandler();

app.Run();