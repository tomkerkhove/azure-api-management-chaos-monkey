using System.Text.Json.Serialization;
using ChaosMonkey.API;
using ChaosMonkey.API.Integrations;
using ChaosMonkey.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables(prefix: "CHAOS_MONKEY_");
builder.Services.AddControllers();
builder.Services.AddSingleton<AzureResourceManagerClient>(services =>
{
    var configuration = services.GetService<IConfiguration>();
    if (configuration == null)
    {
        throw new Exception("No configuration was found");
    }

    var authMode = configuration.GetValue<string>("AUTH_MODE");
    if (authMode == "ClientSecret")
    {
        var tenantId = configuration.GetValue<string>("TENANT_ID");
        return new AzureResourceManagerClient(tenantId, configuration.GetValue<string>("APP_ID"), configuration.GetValue<string>("APP_SECRET"));
    }
    else
    {
        return new AzureResourceManagerClient();
    }
});
builder.Services.AddTransient<ApiManagementRepository>();
builder.Services.AddHostedService<ShutdownGatewayWorker>();

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

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async httpContext =>
    {
        var problemDetailsService = httpContext.RequestServices.GetService<IProblemDetailsService>();
        if (problemDetailsService == null
            || !await problemDetailsService.TryWriteAsync(new() { HttpContext = httpContext }))
        {
            // Fallback behavior
            await httpContext.Response.WriteAsync("Fallback: An error occurred.");
        }
    });
});

app.Run();