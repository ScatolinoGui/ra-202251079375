namespace EstoquePedidos.Api.DTOs;

public record CriarProdutoRequest(
    string Nome,
    string Categoria,
    decimal Preco,
    int Estoque);

public record AtualizarProdutoRequest(
    string Nome,
    string Categoria,
    decimal Preco,
    int Estoque,
    bool Ativo);

public record ProdutoResponse(
    string Id,
    string Nome,
    string Categoria,
    decimal Preco,
    int Estoque,
    bool Ativo,
    DateTime CriadoEm);
