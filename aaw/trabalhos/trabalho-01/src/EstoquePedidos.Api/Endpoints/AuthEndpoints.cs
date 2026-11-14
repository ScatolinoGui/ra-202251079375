using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Services;

namespace EstoquePedidos.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Autenticacao");

        group.MapPost("/login", async (LoginRequest request, AuthService service) =>
        {
            var resposta = await service.LoginAsync(request);
            return Results.Ok(resposta);
        })
        .WithSummary("Autentica um usuario e emite um token JWT Bearer")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/registro", async (RegistroRequest request, AuthService service) =>
        {
            var resposta = await service.RegistrarAsync(request);
            return Results.Created("/api/auth/login", resposta);
        })
        .WithSummary("Registra um novo usuario com senha protegida por PBKDF2 e retorna o token JWT")
        .Produces<AuthResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
