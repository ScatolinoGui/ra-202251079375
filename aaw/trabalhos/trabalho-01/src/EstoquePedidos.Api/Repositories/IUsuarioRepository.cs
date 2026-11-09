using EstoquePedidos.Api.Models;

namespace EstoquePedidos.Api.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);

    Task<Usuario?> ObterPorIdAsync(string id);

    Task<Usuario> CriarAsync(Usuario usuario);

    Task<long> ContarUsuariosAsync();
}
