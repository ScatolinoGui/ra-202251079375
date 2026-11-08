using EscolaApi.Data;
using EscolaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaApi.Controllers;

[ApiController]
[Route("api/v1/alunos")]
public class AlunosController : ControllerBase
{
    private readonly AppDbContext _db;

    public AlunosController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/v1/alunos?page=1&size=10&sort=nome&curso=...
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? sort = null,
        [FromQuery] string? curso = null)
    {
        if (page < 1 || size < 1 || size > 50)
        {
            return Problem(
                title: "Parâmetros de paginação inválidos",
                detail: "Informe page >= 1 e size entre 1 e 50.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        IQueryable<Aluno> query = _db.Alunos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(curso))
        {
            var cursoNormalizado = curso.Trim();
            query = query.Where(a => a.Curso.Contains(cursoNormalizado, StringComparison.OrdinalIgnoreCase));
        }

        query = string.Equals(sort, "nome", StringComparison.OrdinalIgnoreCase)
            ? query.OrderBy(a => a.Nome).ThenBy(a => a.Id)
            : query.OrderBy(a => a.Id);

        var total = await query.CountAsync();
        var itens = await query
            .Skip((page - 1) * size)
            .Take(size)
            .Select(a => new { a.Id, a.Nome, a.Curso })
            .ToListAsync();

        return Ok(new
        {
            page,
            size,
            totalItens = total,
            totalPaginas = (int)Math.Ceiling(total / (double)size),
            itens,
        });
    }

    // GET /api/v1/alunos/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var aluno = await _db.Alunos
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new
            {
                a.Id,
                a.Nome,
                a.Curso,
                Matriculas = a.Matriculas.Select(m => new
                {
                    m.Id,
                    m.Disciplina,
                    m.Semestre,
                    m.Ativa,
                    m.AlunoId,
                }).ToList(),
            })
            .FirstOrDefaultAsync();

        if (aluno is null)
        {
            return Problem(
                title: "Aluno não encontrado",
                detail: $"Não existe aluno cadastrado com id {id}.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(aluno);
    }

    // POST /api/v1/alunos
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarAlunoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Curso))
        {
            return Problem(
                title: "Dados inválidos para cadastro",
                detail: "Os campos Nome e Curso são obrigatórios.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var proximoId = (await _db.Alunos.MaxAsync(a => (int?)a.Id) ?? 0) + 1;
        var aluno = new Aluno
        {
            Id = proximoId,
            Nome = request.Nome.Trim(),
            Curso = request.Curso.Trim(),
        };

        _db.Alunos.Add(aluno);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = aluno.Id },
            new { aluno.Id, aluno.Nome, aluno.Curso });
    }

    // DELETE /api/v1/alunos/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var aluno = await _db.Alunos.FindAsync(id);
        if (aluno is null)
        {
            return Problem(
                title: "Aluno não encontrado",
                detail: $"Não existe aluno cadastrado com id {id}.",
                statusCode: StatusCodes.Status404NotFound);
        }

        _db.Alunos.Remove(aluno);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // GET /api/v1/alunos/{id}/matriculas?semestre=2026.2
    [HttpGet("{id:int}/matriculas")]
    public async Task<IActionResult> GetMatriculas(int id, [FromQuery] string? semestre = null)
    {
        var existe = await _db.Alunos.AnyAsync(a => a.Id == id);
        if (!existe)
        {
            return Problem(
                title: "Aluno não encontrado",
                detail: $"Não existe aluno cadastrado com id {id}.",
                statusCode: StatusCodes.Status404NotFound);
        }

        var query = _db.Matriculas
            .AsNoTracking()
            .Where(m => m.AlunoId == id);

        if (!string.IsNullOrWhiteSpace(semestre))
        {
            var semestreFiltro = semestre.Trim();
            query = query.Where(m => m.Semestre == semestreFiltro);
        }

        var matriculas = await query
            .Select(m => new
            {
                m.Id,
                m.Disciplina,
                m.Semestre,
                m.Ativa,
                m.AlunoId,
            })
            .ToListAsync();

        return Ok(matriculas);
    }

    // GET /api/v1/alunos/{id}/matriculas/{matriculaId}
    [HttpGet("{id:int}/matriculas/{matriculaId:int}")]
    public async Task<IActionResult> GetMatricula(int id, int matriculaId)
    {
        var matricula = await _db.Matriculas
            .AsNoTracking()
            .Where(m => m.Id == matriculaId && m.AlunoId == id)
            .Select(m => new
            {
                m.Id,
                m.Disciplina,
                m.Semestre,
                m.Ativa,
                m.AlunoId,
            })
            .FirstOrDefaultAsync();

        if (matricula is null)
        {
            return Problem(
                title: "Matrícula não encontrada",
                detail: $"Matrícula {matriculaId} não encontrada para o aluno {id}.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(matricula);
    }

    public record CriarAlunoRequest(string Nome, string Curso);
}
