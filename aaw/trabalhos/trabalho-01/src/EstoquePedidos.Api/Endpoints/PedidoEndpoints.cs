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
        .RequireAuthorization();

        group.MapGet("/{id}", async (string id, PedidoService service) =>
        {
            var pedido = await service.ObterPorIdAsync(id);
            return Results.Ok(pedido);
        })
        .RequireAuthorization();

        group.MapPost("/", async (CriarPedidoRequest request, PedidoService service) =>
        {
            var criado = await service.CriarAsync(request);
            return Results.Created($"/api/pedidos/{criado.Id}", criado);
        })
        .RequireAuthorization();

        group.MapDelete("/{id}", async (string id, PedidoService service) =>
        {
            var cancelado = await service.CancelarAsync(id);
            return Results.Ok(cancelado);
        })
        .RequireAuthorization(p => p.RequireRole("Admin"));

        // Sub-recurso do relacionamento 1-N: pedidos de um produto especifico (rota publica)
        app.MapGet("/api/produtos/{id}/pedidos", async (string id, PedidoService service) =>
        {
            var pedidos = await service.ListarPorProdutoIdAsync(id);
            return Results.Ok(pedidos);
        })
        .WithTags("Produtos");
    }
}
