using EstoquePedidos.Api.Config;
using EstoquePedidos.Api.Endpoints;
using EstoquePedidos.Api.Repositories;
using EstoquePedidos.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<PedidoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "online",
    servico = "EstoquePedidos.Api",
    timestamp = DateTime.UtcNow
}));

app.MapProdutoEndpoints();

app.Run();

public partial class Program { }
