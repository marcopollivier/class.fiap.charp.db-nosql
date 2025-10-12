using PedidosApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO DOS SERVIÇOS =====

// Configuração dos bancos de dados
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddSqlServer(builder.Configuration);

// Configuração dos serviços da API
builder.Services.AddApiServices();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// ===== CONFIGURAÇÃO DO PIPELINE =====

// Inicialização dos bancos de dados
app.InitializeSqlServerDatabase();

// Pipeline de desenvolvimento
app.ConfigureDevelopmentPipeline();

// Pipeline da aplicação
app.ConfigureApplicationPipeline();

app.Run();
