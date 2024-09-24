using Serilog;
using Microsoft.EntityFrameworkCore;
using VendasAPI.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configurando Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();


// Configurando o contexto de dados com SQLite
builder.Services.AddDbContext<VendasContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("VendasDatabase")));

// Configurando os servi�os da aplica��o
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

// Usando Serilog para logar requisi��es
app.UseSerilogRequestLogging();

app.Run();