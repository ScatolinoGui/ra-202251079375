var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "online",
    servico = "EstoquePedidos.Api",
    timestamp = DateTime.UtcNow
}));

app.Run();

public partial class Program { }
