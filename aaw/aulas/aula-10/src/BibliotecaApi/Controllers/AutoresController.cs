using BibliotecaApi.Data;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Autores")]
public class AutoresController : ControllerBase
{
    private readonly BibliotecaRepository _repo;

    public AutoresController(BibliotecaRepository repo) => _repo = repo;

    /// <summary>
    /// Lista todos os autores cadastrados.
    /// </summary>
    /// <returns>Lista de autores com seus livros associados.</returns>
    /// <response code="200">Retorna a lista de autores.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Autor>), StatusCodes.Status200OK)]
    public IActionResult ObterTodos()
    {
        return Ok(_repo.ObterAutores());
    }

    /// <summary>
    /// Obtem um autor especifico pelo seu ID.
    /// </summary>
    /// <param name="id">ID do autor.</param>
    /// <returns>Dados do autor encontrado.</returns>
    /// <response code="200">Autor encontrado.</response>
    /// <response code="404">Autor nao encontrado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Autor), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult ObterPorId(int id)
    {
        var autor = _repo.ObterAutorPorId(id);
        if (autor == null)
            return NotFound(new { mensagem = $"Autor com ID {id} nao encontrado." });
        return Ok(autor);
    }

    /// <summary>
    /// Cadastra um novo autor.
    /// </summary>
    /// <param name="autor">Dados do autor a ser cadastrado.</param>
    /// <returns>Autor recem-criado.</returns>
    /// <response code="201">Autor criado com sucesso.</response>
    /// <response code="400">Dados invalidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Autor), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Criar([FromBody] Autor autor)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var criado = _repo.CriarAutor(autor);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>
    /// Atualiza um autor existente.
    /// </summary>
    /// <param name="id">ID do autor a atualizar.</param>
    /// <param name="autor">Novos dados do autor.</param>
    /// <response code="204">Atualizado com sucesso.</response>
    /// <response code="404">Autor nao encontrado.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Atualizar(int id, [FromBody] Autor autor)
    {
        if (!_repo.AtualizarAutor(id, autor))
            return NotFound(new { mensagem = $"Autor com ID {id} nao encontrado." });
        return NoContent();
    }

    /// <summary>
    /// Remove um autor do acervo.
    /// </summary>
    /// <param name="id">ID do autor a remover.</param>
    /// <response code="204">Removido com sucesso.</response>
    /// <response code="404">Autor nao encontrado.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Remover(int id)
    {
        if (!_repo.RemoverAutor(id))
            return NotFound(new { mensagem = $"Autor com ID {id} nao encontrado." });
        return NoContent();
    }
}
