namespace EstoquePedidos.Api.DTOs;

public record CriarPedidoRequest(
    string ProdutoId,
    string NomeCliente,
    int Quantidade,
    string? Cupom);

public record PedidoResponse(
    string Id,
    string ProdutoId,
    string NomeProduto,
    string NomeCliente,
    int Quantidade,
    string? Cupom,
    decimal Subtotal,
    decimal Desconto,
    decimal Frete,
    decimal ValorTotal,
    string Status,
    DateTime CriadoEm);
