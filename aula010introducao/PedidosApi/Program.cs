using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using PedidosApi.Data;
using PedidosApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// MongoDB
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = "mongodb://admin:password123@localhost:27017";
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("pedidos");
});

builder.Services.AddScoped<MongoRepository>();

// SQL Server
builder.Services.AddDbContext<SqlContext>(options =>
    options.UseSqlServer("Server=localhost,1433;Database=Pedidos;User Id=sa;Password=Password123!;TrustServerCertificate=true"));

builder.Services.AddScoped<SqlRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Criar banco SQL Server
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SqlContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
