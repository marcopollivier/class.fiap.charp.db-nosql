using PedidosApi.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adicionar serviços Redis
builder.Services.AddRedisServices(builder.Configuration);

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<RedisHealthCheck>("redis");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Redis Examples API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Endpoint de health check
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                exception = e.Value.Exception?.Message,
                duration = e.Value.Duration
            })
        });
        await context.Response.WriteAsync(result);
    }
});

// Endpoint de informações básicas
app.MapGet("/", () => new
{
    message = "Redis Examples API - Aula 04",
    endpoints = new[]
    {
        "/swagger - Documentação da API",
        "/health - Status dos serviços",
        "/api/redisexamples - Exemplos práticos do Redis"
    },
    examples = new[]
    {
        "GET /api/redisexamples/cache-produto/123 - Cache básico",
        "POST /api/redisexamples/incrementar-visitas/123 - Contador",
        "GET /api/redisexamples/leaderboard/top/10 - Rankings",
        "GET /api/redisexamples/redis-info - Informações do Redis"
    }
});

app.Run();
