using System.Text;
using EstoquePedidos.Api.Config;
using EstoquePedidos.Api.Endpoints;
using EstoquePedidos.Api.Repositories;
using EstoquePedidos.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DatabaseSeeder>();

// Configuracao do JWT (pode ser sobrescrita por variavel de ambiente JwtSettings__SecretKey etc.)
var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

// HMAC-SHA256 exige chave de pelo menos 256 bits; falha na subida em vez de falhar no primeiro login
if (Encoding.UTF8.GetByteCount(jwtSettings.SecretKey) < 32)
{
    throw new InvalidOperationException(
        "JwtSettings:SecretKey precisa ter pelo menos 32 bytes.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };

        // Padroniza 401 e 403 do pipeline JWT em application/problem+json (RFC 9457 / RFC 7807)
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await Results.Problem(
                    title: "Nao autenticado",
                    detail: "Token JWT ausente, invalido ou expirado.",
                    statusCode: StatusCodes.Status401Unauthorized,
                    type: "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                    instance: context.Request.Path)
                    .ExecuteAsync(context.HttpContext);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await Results.Problem(
                    title: "Acesso negado",
                    detail: "Seu perfil de usuario nao possui permissao para executar esta operacao.",
                    statusCode: StatusCodes.Status403Forbidden,
                    type: "https://tools.ietf.org/html/rfc9110#section-15.5.4",
                    instance: context.Request.Path)
                    .ExecuteAsync(context.HttpContext);
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Controle de Estoque e Pedidos API",
        Version = "v1",
        Description = "API RESTful para gestao de catalogo de produtos, emissao de pedidos com controle de estoque, descontos, calculo de frete e controle de acesso RBAC."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT obtido em POST /api/auth/login."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseStatusCodePages();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Controle de Estoque e Pedidos API v1");
});

// Autenticacao identifica quem e o usuario; autorizacao decide o que ele pode fazer
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "online",
    servico = "EstoquePedidos.Api",
    timestamp = DateTime.UtcNow
}))
.WithTags("Sistema")
.WithSummary("Verifica a disponibilidade da API");

app.MapAuthEndpoints();
app.MapProdutoEndpoints();
app.MapPedidoEndpoints();

try
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "Seed inicial ignorado porque o MongoDB nao esta acessivel no momento.");
}

app.Run();

public partial class Program { }
