using BibliotecaApi.Models;
using BibliotecaApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LivrosController : ControllerBase
{
    private readonly LivroRepository _repository;

    public LivrosController(LivroRepository repository)
    {
        _repository = repository;
    }

    // PASSO 1: GET api/livros -> 200 OK
    [HttpGet]
    public ActionResult<List<Livro>> GetAll()
    {
        return Ok(_repository.GetAll());
    }

    // PASSO 2: GET api/livros/{id} -> 200 OK | 404 Not Found
    [HttpGet("{id}")]
    public ActionResult<Livro> GetById(int id)
    {
        var livro = _repository.GetById(id);
        if (livro is null)
        {
            return NotFound();
        }

        return Ok(livro);
    }

    // PASSO 3: POST api/livros -> 201 Created + Location | 400 Bad Request
    [HttpPost]
    public ActionResult<Livro> Create(Livro livro)
    {
        var criado = _repository.Create(livro);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    // PASSO 4: PUT api/livros/{id} -> 200 OK | 404 Not Found
    [HttpPut("{id}")]
    public ActionResult<Livro> Update(int id, Livro livro)
    {
        var atualizado = _repository.Update(id, livro);
        if (atualizado is null)
        {
            return NotFound();
        }

        return Ok(atualizado);
    }

    // PASSO 5: DELETE api/livros/{id} -> 204 No Content | 404 Not Found
    // Idempotencia: a primeira chamada remove (204) e as seguintes retornam 404 sem alterar o estado.
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var removido = _repository.Delete(id);
        if (!removido)
        {
            return NotFound();
        }

        return NoContent();
    }
}
