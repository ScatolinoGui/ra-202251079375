using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Models;

namespace EstoquePedidos.Api.Repositories;

public interface IPedidoRepository
{
    Task<PagedResult<Pedido>> ListarAsync(string? status, int page, int pageSize);

    Task<List<Pedido>> ListarPorProdutoIdAsync(string produtoId);

    Task<Pedido?> ObterPorIdAsync(string id);

    Task<Pedido> CriarAsync(Pedido pedido);

    Task<bool> AtualizarAsync(string id, Pedido pedido);

    Task<bool> ExistePedidoParaProdutoAsync(string produtoId);
}
