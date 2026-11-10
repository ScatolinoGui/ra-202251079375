using ApiVazada.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiVazada.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProdutosController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult GetAll() => Ok(_db.Produtos.ToList());

    [HttpGet("buscar")]
    public IActionResult Buscar(string nome)
    {
        var resultado = _db.Produtos
            .FromSqlRaw("SELECT * FROM Produtos WHERE Nome LIKE {0}", $"%{nome}%")
            .ToList();
        return Ok(resultado);
    }

    [HttpGet("quebrar")]
    public IActionResult Quebrar()
    {
        throw new InvalidOperationException(
            "Erro proposital para demonstrar a exposicao de stack trace.");
    }
}
