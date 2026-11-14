using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Services;

namespace EstoquePedidos.Api.Endpoints;

public static class PedidoEndpoints
{
    public static void MapPedidoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/pedidos")
            .WithTags("Pedidos");

        group.MapGet("/", async (
            string? status,
            int? page,
            int? pageSize,
            PedidoService service) =>
        {
            var resultado = await service.ListarAsync(
                status,
                page ?? 1,
                pageSize ?? 10);
            return Results.Ok(resultado);
        })
        .RequireAuthorization()
        .WithSummary("Lista pedidos com filtro opcional por status e paginacao (requer login)")
        .Produces<PagedResult<PedidoResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/{id}", async (string id, PedidoService service) =>
        {
            var pedido = await service.ObterPorIdAsync(id);
            return Results.Ok(pedido);
        })
        .RequireAuthorization()
        .WithSummary("Obtem os detalhes de um pedido pelo seu identificador (requer login)")
        .Produces<PedidoResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CriarPedidoRequest request, PedidoService service) =>
        {
            var criado = await service.CriarAsync(request);
            return Results.Created($"/api/pedidos/{criado.Id}", criado);
        })
        .RequireAuthorization()
        .WithSummary("Emite um novo pedido calculando desconto, frete e baixando o estoque (requer login)")
        .Produces<PedidoResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id}", async (string id, PedidoService service) =>
        {
            var cancelado = await service.CancelarAsync(id);
            return Results.Ok(cancelado);
        })
        .RequireAuthorization(p => p.RequireRole("Admin"))
        .WithSummary("Cancela um pedido confirmado e estorna a quantidade ao estoque do produto (exclusivo Admin)")
        .Produces<PedidoResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        // Sub-recurso do relacionamento 1-N: pedidos de um produto especifico (rota publica)
        app.MapGet("/api/produtos/{id}/pedidos", async (string id, PedidoService service) =>
        {
            var pedidos = await service.ListarPorProdutoIdAsync(id);
            return Results.Ok(pedidos);
        })
        .WithTags("Produtos")
        .WithSummary("Lista todos os pedidos vinculados a um produto especifico (relacionamento 1-N)")
        .Produces<List<PedidoResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
