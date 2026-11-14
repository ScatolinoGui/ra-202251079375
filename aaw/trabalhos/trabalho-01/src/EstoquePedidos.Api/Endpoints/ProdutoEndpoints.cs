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
        })
        .WithSummary("Lista produtos do catalogo com filtro por categoria, busca por nome e paginacao")
        .Produces<PagedResult<ProdutoResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id}", async (string id, ProdutoService service) =>
        {
            var produto = await service.ObterPorIdAsync(id);
            return Results.Ok(produto);
        })
        .WithSummary("Obtem os detalhes de um produto pelo seu identificador")
        .Produces<ProdutoResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CriarProdutoRequest request, ProdutoService service) =>
        {
            var criado = await service.CriarAsync(request);
            return Results.Created($"/api/produtos/{criado.Id}", criado);
        })
        .RequireAuthorization(p => p.RequireRole("Admin"))
        .WithSummary("Cadastra um novo produto no catalogo (exclusivo Admin)")
        .Produces<ProdutoResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id}", async (string id, AtualizarProdutoRequest request, ProdutoService service) =>
        {
            var atualizado = await service.AtualizarAsync(id, request);
            return Results.Ok(atualizado);
        })
        .RequireAuthorization(p => p.RequireRole("Admin"))
        .WithSummary("Atualiza os dados e o estoque de um produto existente (exclusivo Admin)")
        .Produces<ProdutoResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id}", async (string id, ProdutoService service) =>
        {
            await service.RemoverAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(p => p.RequireRole("Admin"))
        .WithSummary("Remove um produto sem pedidos vinculados (exclusivo Admin)")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
