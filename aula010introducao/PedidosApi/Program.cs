using PedidosApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddSqlServer(builder.Configuration);
builder.Services.AddApiServices();

var app = builder.Build();

app.InitializeSqlServerDatabase();
app.ConfigureApplicationPipeline();

app.Run();
