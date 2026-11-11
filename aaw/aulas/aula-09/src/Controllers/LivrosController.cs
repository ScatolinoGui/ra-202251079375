using BibliotecaApi.Data;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LivrosController : ControllerBase
{
    private readonly BibliotecaStore _store;

    public LivrosController(BibliotecaStore store) => _store = store;

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<Livro>> Listar() => Ok(_store.ListarLivros());

    [AllowAnonymous]
    [HttpGet("{id}")]
    public ActionResult<Livro> BuscarPorId(int id)
    {
        var livro = _store.BuscarLivro(id);
        return livro is null ? NotFound() : Ok(livro);
    }

    [Authorize(Policy = "GerenciarAcervo")]
    [HttpPost]
    public ActionResult<Livro> Criar([FromBody] LivroRequest dados)
    {
        if (string.IsNullOrWhiteSpace(dados.Titulo))
            return BadRequest(new { message = "Titulo e obrigatorio." });

        var livro = _store.AdicionarLivro(dados);
        return CreatedAtAction(nameof(BuscarPorId), new { id = livro.Id }, livro);
    }

    [Authorize(Policy = "GerenciarAcervo")]
    [HttpPut("{id}")]
    public IActionResult Atualizar(int id, [FromBody] LivroRequest dados)
    {
        if (string.IsNullOrWhiteSpace(dados.Titulo))
            return BadRequest(new { message = "Titulo e obrigatorio." });

        return _store.AtualizarLivro(id, dados) ? NoContent() : NotFound();
    }

    [Authorize(Policy = "RemoverLivro")]
    [HttpDelete("{id}")]
    public IActionResult Remover(int id) =>
        _store.RemoverLivro(id) ? NoContent() : NotFound();
}
