using PedidosApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddApiServices();

// Adicionar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Pedidos API - MongoDB",
        Version = "v1",
        Description = "API demonstrativa para Aula 021 - MongoDB Aplicado"
    });
});

var app = builder.Build();

// Configurar pipeline da aplicação
app.ConfigureApplicationPipeline();

app.Run();
