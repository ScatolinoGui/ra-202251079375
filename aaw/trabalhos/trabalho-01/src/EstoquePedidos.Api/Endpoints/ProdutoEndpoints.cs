using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Services;

namespace EstoquePedidos.Api.Endpoints;

public static class ProdutoEndpoints
{
    public static void MapProdutoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/produtos")
            .WithTags("Produtos");

        group.MapGet("/", async (
            string? categoria,
            string? busca,
            int? page,
            int? pageSize,
            ProdutoService service) =>
        {
            var resultado = await service.ListarAsync(
                categoria,
                busca,
                page ?? 1,
                pageSize ?? 10);
            return Results.Ok(resultado);
        });

        group.MapGet("/{id}", async (string id, ProdutoService service) =>
        {
            var produto = await service.ObterPorIdAsync(id);
            return Results.Ok(produto);
        });

        group.MapPost("/", async (CriarProdutoRequest request, ProdutoService service) =>
        {
            var criado = await service.CriarAsync(request);
            return Results.Created($"/api/produtos/{criado.Id}", criado);
        })
        .RequireAuthorization(p => p.RequireRole("Admin"));

        group.MapPut("/{id}", async (string id, AtualizarProdutoRequest request, ProdutoService service) =>
        {
            var atualizado = await service.AtualizarAsync(id, request);
            return Results.Ok(atualizado);
        })
        .RequireAuthorization(p => p.RequireRole("Admin"));

        group.MapDelete("/{id}", async (string id, ProdutoService service) =>
        {
            await service.RemoverAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(p => p.RequireRole("Admin"));
    }
}
