using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Models;

namespace EstoquePedidos.Api.Repositories;

public interface IProdutoRepository
{
    Task<PagedResult<Produto>> ListarAsync(string? categoria, string? busca, int page, int pageSize);

    Task<Produto?> ObterPorIdAsync(string id);

    Task<Produto?> ObterPorNomeAsync(string nome);

    Task<Produto> CriarAsync(Produto produto);

    Task<bool> AtualizarAsync(string id, Produto produto);

    Task<bool> RemoverAsync(string id);

    Task<long> ContarAsync();
}
