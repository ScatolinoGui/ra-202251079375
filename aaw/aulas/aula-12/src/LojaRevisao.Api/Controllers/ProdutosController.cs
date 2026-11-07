using LojaRevisao.Api.Data;
using LojaRevisao.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaRevisao.Api.Controllers;

[ApiController]
[Route("api/v1/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly LojaDbContext _db;

    public ProdutosController(LojaDbContext db) => _db = db;

    // Paginacao ordenada por Id com limite maximo de 100 itens por pagina imposto no servidor.
    [HttpGet]
    public IActionResult Listar([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        page = Math.Max(page, 1);
        size = Math.Clamp(size, 1, 100);

        var total = _db.Produtos.Count();
        var itens = _db.Produtos
            .OrderBy(p => p.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        return Ok(new Pagina<Produto>(page, size, total, itens));
    }

    // Consulta parametrizada para evitar SQL Injection na busca por nome.
    [HttpGet("busca")]
    public IActionResult Buscar([FromQuery] string nome)
    {
        var resultado = _db.Produtos
            .FromSqlRaw("SELECT * FROM Produtos WHERE Nome LIKE {0}", $"%{nome}%")
            .Take(50)
            .ToList();
        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    public IActionResult Obter(int id)
    {
        var produto = _db.Produtos.Find(id);
        if (produto is null) return NotFound();
        return Ok(produto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Criar(NovoProduto novo)
    {
        var produto = new Produto { Nome = novo.Nome, Preco = novo.Preco, Estoque = novo.Estoque };
        _db.Produtos.Add(produto);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Obter), new { id = produto.Id }, produto);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Remover(int id)
    {
        var produto = _db.Produtos.Find(id);
        if (produto is null) return NotFound();

        _db.Produtos.Remove(produto);
        _db.SaveChanges();
        return NoContent();
    }
}
