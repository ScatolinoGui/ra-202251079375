using BibliotecaApi.Data;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Emprestimos")]
public class EmprestimosController : ControllerBase
{
    private readonly BibliotecaRepository _repo;

    public EmprestimosController(BibliotecaRepository repo) => _repo = repo;

    /// <summary>
    /// Lista todos os emprestimos registrados.
    /// </summary>
    /// <response code="200">Lista de emprestimos retornada com sucesso.</response>
    /// <response code="401">Usuario nao autenticado.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Emprestimo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ObterTodos()
    {
        return Ok(_repo.ObterEmprestimos());
    }

    /// <summary>
    /// Obtem um emprestimo especifico por ID.
    /// </summary>
    /// <param name="id">ID do emprestimo.</param>
    /// <response code="200">Emprestimo encontrado.</response>
    /// <response code="401">Usuario nao autenticado.</response>
    /// <response code="404">Emprestimo nao encontrado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Emprestimo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult ObterPorId(int id)
    {
        var emprestimo = _repo.ObterEmprestimoPorId(id);
        if (emprestimo == null)
            return NotFound(new { mensagem = $"Emprestimo com ID {id} nao encontrado." });
        return Ok(emprestimo);
    }

    /// <summary>
    /// Realiza um novo emprestimo de livro.
    /// </summary>
    /// <remarks>
    /// O livro deve estar disponivel (nao emprestado).
    /// O prazo de devolucao padrao e de 14 dias.
    ///
    /// Exemplo de requisicao:
    ///
    ///     POST /api/emprestimos
    ///     {
    ///         "livroId": 1,
    ///         "nomeUsuario": "Joao Silva"
    ///     }
    ///
    /// </remarks>
    /// <param name="emprestimo">Dados do emprestimo.</param>
    /// <response code="201">Emprestimo criado com sucesso.</response>
    /// <response code="400">Dados invalidos.</response>
    /// <response code="401">Usuario nao autenticado.</response>
    /// <response code="409">Livro indisponivel para emprestimo.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Emprestimo), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Criar([FromBody] Emprestimo emprestimo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var criado = _repo.CriarEmprestimo(emprestimo);
        if (criado == null)
            return Conflict(new { mensagem = "Livro nao encontrado ou nao esta disponivel para emprestimo." });

        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>
    /// Registra a devolucao de um livro emprestado.
    /// </summary>
    /// <param name="id">ID do emprestimo.</param>
    /// <response code="200">Livro devolvido com sucesso.</response>
    /// <response code="401">Usuario nao autenticado.</response>
    /// <response code="404">Emprestimo nao encontrado ou ja devolvido.</response>
    [HttpPatch("{id:int}/devolver")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Devolver(int id)
    {
        if (!_repo.DevolverLivro(id))
            return NotFound(new { mensagem = "Emprestimo nao encontrado ou ja devolvido." });

        var emprestimo = _repo.ObterEmprestimoPorId(id);
        return Ok(new { mensagem = "Livro devolvido com sucesso.", emprestimo });
    }
}
