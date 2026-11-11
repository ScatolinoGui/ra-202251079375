using BibliotecaApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "VerRelatorios")]
public class RelatoriosController : ControllerBase
{
    private readonly BibliotecaStore _store;

    public RelatoriosController(BibliotecaStore store) => _store = store;

    [HttpGet("leitores")]
    public IActionResult Leitores() =>
        Ok(BibliotecaStore.Usuarios.Select(u => new { u.Id, u.Nome, u.Email, u.Telefone, u.Cpf, u.Role }));

    [HttpGet("emprestimos")]
    public IActionResult Emprestimos() =>
        Ok(_store.ListarEmprestimos().Select(e => new
        {
            e.Id,
            Livro = _store.BuscarLivro(e.LivroId)?.Titulo,
            Usuario = BibliotecaStore.Usuarios.First(u => u.Id == e.UsuarioId).Nome,
            e.Devolucao
        }));
}
