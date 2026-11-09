using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using PetShopApi.Data;
using PetShopApi.Models;

namespace PetShopApi.Controllers;

/// <summary>
/// API do PetHouse refatorada com os 12 erros de design REST corrigidos.
/// </summary>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public class PetsController : ControllerBase
{
    private const int TamanhoPadraoDaPagina = 20;
    private const int TamanhoMaximoDaPagina = 100;

    private readonly PetShopStore _store;

    public PetsController(PetShopStore store)
    {
        _store = store;
    }

    // ---------------------------------------------------------------- PETS
    // Corrige os erros 01 (verbo na URI e leitura com POST), 03 (recurso no singular)
    // e 09 (listagem sem paginação).

    /// <summary>Lista os pets com paginação e filtros opcionais.</summary>
    [HttpGet("pets", Name = "ListarPets")]
    public ActionResult<Pagina<Pet>> ListarPets(
        [FromQuery] int page = 1,
        [FromQuery] int size = TamanhoPadraoDaPagina,
        [FromQuery] string? especie = null,
        [FromQuery] int? tutorId = null)
    {
        if (page < 1 || size < 1 || size > TamanhoMaximoDaPagina) return PaginacaoInvalida();

        IEnumerable<Pet> consulta = _store.Pets;

        if (!string.IsNullOrWhiteSpace(especie))
            consulta = consulta.Where(p => p.Especie.Equals(especie, StringComparison.OrdinalIgnoreCase));

        if (tutorId is not null)
            consulta = consulta.Where(p => p.TutorId == tutorId.Value);

        var todos = consulta.OrderBy(p => p.Id).ToList();

        return Ok(new Pagina<Pet>
        {
            Page = page,
            Size = size,
            Total = todos.Count,
            Items = todos.Skip((page - 1) * size).Take(size).ToList()
        });
    }

    /// <summary>Obtém um pet pelo id.</summary>
    // Corrige os erros 03 (substantivo no plural) e 06 (404 Not Found com ProblemDetails).
    [HttpGet("pets/{id:int}", Name = "ObterPet")]
    public ActionResult<Pet> ObterPet(int id)
    {
        var pet = _store.BuscarPet(id);
        if (pet is null) return PetNaoEncontrado(id);

        return Ok(pet);
    }

    /// <summary>Cadastra um pet.</summary>
    // Corrige o erro 05: retorna 201 Created com cabeçalho Location apontando para o novo recurso.
    [HttpPost("pets", Name = "CadastrarPet")]
    public ActionResult<Pet> CadastrarPet([FromBody] Pet pet)
    {
        var criado = _store.CriarPet(pet);

        return CreatedAtAction(nameof(ObterPet), new { id = criado.Id }, criado);
    }

    /// <summary>Remove um pet.</summary>
    // Corrige o erro 02: remove o verbo da URI, utiliza DELETE e devolve 204 No Content ou 404 Not Found.
    [HttpDelete("pets/{id:int}", Name = "RemoverPet")]
    public IActionResult RemoverPet(int id)
    {
        if (!_store.RemoverPet(id)) return PetNaoEncontrado(id);

        return NoContent();
    }

    // ------------------------------------------------------------- VACINAS
    // Corrige o erro 10 separando as semânticas:
    // POST registra nova aplicação (evento não idempotente);
    // PUT substitui a carteira inteira (operação idempotente).

    /// <summary>Lista a carteira de vacinação do pet.</summary>
    [HttpGet("pets/{id:int}/vacinas", Name = "ListarVacinas")]
    public ActionResult<List<Vacina>> ListarVacinas(int id)
    {
        if (_store.BuscarPet(id) is null) return PetNaoEncontrado(id);

        return Ok(_store.VacinasDoPet(id));
    }

    /// <summary>Registra a aplicação de uma vacina.</summary>
    [HttpPost("pets/{id:int}/vacinas", Name = "AplicarVacina")]
    public ActionResult<Vacina> AplicarVacina(int id, [FromBody] Vacina vacina)
    {
        if (_store.BuscarPet(id) is null) return PetNaoEncontrado(id);

        var criada = _store.RegistrarVacina(id, string.IsNullOrWhiteSpace(vacina.Nome) ? "V10" : vacina.Nome);

        return CreatedAtAction(nameof(ListarVacinas), new { id }, criada);
    }

    /// <summary>Substitui a carteira de vacinação inteira de forma idempotente.</summary>
    [HttpPut("pets/{id:int}/vacinas", Name = "SubstituirCarteira")]
    public ActionResult<List<Vacina>> SubstituirCarteira(int id, [FromBody] List<Vacina> carteira)
    {
        if (_store.BuscarPet(id) is null) return PetNaoEncontrado(id);

        _store.SubstituirCarteira(id, carteira);

        return Ok(_store.VacinasDoPet(id));
    }

    // ------------------------------------------------ TUTORES E ATENDIMENTOS
    // Corrige o erro 04: padroniza URIs em kebab-case e move "vip" para filtro de query string.

    /// <summary>Lista os tutores com filtro opcional por programa de fidelidade (?vip=true).</summary>
    [HttpGet("tutores", Name = "ListarTutores")]
    public ActionResult<Pagina<Tutor>> ListarTutores(
        [FromQuery] bool? vip = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = TamanhoPadraoDaPagina)
    {
        if (page < 1 || size < 1 || size > TamanhoMaximoDaPagina) return PaginacaoInvalida();

        IEnumerable<Tutor> consulta = _store.Tutores;
        if (vip is not null) consulta = consulta.Where(t => t.Vip == vip.Value);

        var todos = consulta.OrderBy(t => t.Id).ToList();

        return Ok(new Pagina<Tutor>
        {
            Page = page,
            Size = size,
            Total = todos.Count,
            Items = todos.Skip((page - 1) * size).Take(size).ToList()
        });
    }

    /// <summary>Lista os pets de um tutor específico.</summary>
    // Corrige o erro 11: elimina estado de sessão estático no servidor (stateless).
    [HttpGet("tutores/{tutorId:int}/pets", Name = "PetsDoTutor")]
    public ActionResult<List<Pet>> PetsDoTutor(int tutorId)
    {
        if (!_store.Tutores.Any(t => t.Id == tutorId))
        {
            return Problem(
                title: "Tutor não encontrado.",
                detail: $"Não existe tutor com id {tutorId}.",
                statusCode: StatusCodes.Status404NotFound,
                instance: $"/api/v1/tutores/{tutorId}");
        }

        return Ok(_store.Pets.Where(p => p.TutorId == tutorId).ToList());
    }

    /// <summary>Lista os atendimentos de banho e tosa.</summary>
    [HttpGet("banhos-e-tosas", Name = "ListarBanhosETosas")]
    public ActionResult<Pagina<BanhoTosa>> ListarBanhosETosas(
        [FromQuery] int page = 1,
        [FromQuery] int size = TamanhoPadraoDaPagina,
        [FromQuery] int? petId = null)
    {
        if (page < 1 || size < 1 || size > TamanhoMaximoDaPagina) return PaginacaoInvalida();

        IEnumerable<BanhoTosa> consulta = _store.BanhosETosas;
        if (petId is not null) consulta = consulta.Where(b => b.PetId == petId.Value);

        var todos = consulta.OrderBy(b => b.Id).ToList();

        return Ok(new Pagina<BanhoTosa>
        {
            Page = page,
            Size = size,
            Total = todos.Count,
            Items = todos.Skip((page - 1) * size).Take(size).ToList()
        });
    }

    // --------------------------------------------- CONSULTAS E EXAMES
    // Corrige os erros 08 (aninhamento profundo de 5 níveis) e 09 (coleção sem paginação).

    /// <summary>Lista as consultas com paginação, filtros e ordenação.</summary>
    [HttpGet("consultas", Name = "ListarConsultas")]
    public ActionResult<Pagina<Consulta>> ListarConsultas(
        [FromQuery] int page = 1,
        [FromQuery] int size = TamanhoPadraoDaPagina,
        [FromQuery] int? petId = null,
        [FromQuery] string? veterinario = null,
        [FromQuery] string? sort = null)
    {
        if (page < 1 || size < 1 || size > TamanhoMaximoDaPagina) return PaginacaoInvalida();

        IEnumerable<Consulta> consulta = _store.Consultas;

        if (petId is not null)
            consulta = consulta.Where(c => c.PetId == petId.Value);

        if (!string.IsNullOrWhiteSpace(veterinario))
            consulta = consulta.Where(c => c.Veterinario.Contains(veterinario, StringComparison.OrdinalIgnoreCase));

        consulta = sort switch
        {
            "data" => consulta.OrderBy(c => c.Data),
            "-data" => consulta.OrderByDescending(c => c.Data),
            _ => consulta.OrderBy(c => c.Id)
        };

        var todos = consulta.ToList();

        return Ok(new Pagina<Consulta>
        {
            Page = page,
            Size = size,
            Total = todos.Count,
            Items = todos.Skip((page - 1) * size).Take(size).ToList()
        });
    }

    /// <summary>Lista os exames de uma consulta (máximo 1 nível de aninhamento).</summary>
    [HttpGet("consultas/{consultaId:int}/exames", Name = "ExamesDaConsulta")]
    public ActionResult<List<Exame>> ExamesDaConsulta(int consultaId)
    {
        if (!_store.Consultas.Any(c => c.Id == consultaId))
        {
            return Problem(
                title: "Consulta não encontrada.",
                detail: $"Não existe consulta com id {consultaId}.",
                statusCode: StatusCodes.Status404NotFound,
                instance: $"/api/v1/consultas/{consultaId}");
        }

        return Ok(_store.Exames.Where(e => e.ConsultaId == consultaId).ToList());
    }

    /// <summary>Obtém um exame pelo id como recurso de primeiro nível.</summary>
    [HttpGet("exames/{id:int}", Name = "ObterExame")]
    public ActionResult<Exame> ObterExame(int id)
    {
        var exame = _store.Exames.FirstOrDefault(e => e.Id == id);
        if (exame is null)
        {
            return Problem(
                title: "Exame não encontrado.",
                detail: $"Não existe exame com id {id}.",
                statusCode: StatusCodes.Status404NotFound,
                instance: $"/api/v1/exames/{id}");
        }

        return Ok(exame);
    }

    // ------------------------------------------------ TABELA DE PREÇOS
    // Corrige o erro 12: habilita cache com Cache-Control, ETag e validação condicional 304 Not Modified.

    /// <summary>Tabela de preços dos serviços com suporte a cache condicional.</summary>
    [HttpGet("tabela-de-precos", Name = "TabelaDePrecos")]
    public IActionResult TabelaDePrecos()
    {
        var etag = $"\"precos-v{_store.VersaoDaTabelaDePrecos}\"";

        if (Request.Headers[HeaderNames.IfNoneMatch].ToString()
                   .Split(',').Select(v => v.Trim()).Contains(etag))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        Response.Headers[HeaderNames.ETag] = etag;
        Response.Headers[HeaderNames.CacheControl] = "public, max-age=3600";

        return Ok(_store.TabelaDePrecos);
    }

    // ------------------------------------------------------------ HELPERS

    private ObjectResult PetNaoEncontrado(int id) => Problem(
        title: "Pet não encontrado.",
        detail: $"Não existe pet com id {id}.",
        statusCode: StatusCodes.Status404NotFound,
        instance: $"/api/v1/pets/{id}");

    private ObjectResult PaginacaoInvalida() => Problem(
        title: "Parâmetro de paginação inválido.",
        detail: $"'page' começa em 1 e 'size' deve estar entre 1 e {TamanhoMaximoDaPagina}.",
        statusCode: StatusCodes.Status400BadRequest);
}

/// <summary>
/// Corrige o erro 07: quando há quebra de contrato (renomear "nome" para "nomeDoPet"),
/// preserva-se a rota /api/v1/pets intacta e publica-se a mudança em /api/v2/pets.
/// </summary>
[ApiController]
[Route("api/v2")]
[Produces("application/json")]
public class PetsV2Controller : ControllerBase
{
    private const int TamanhoPadraoDaPagina = 20;
    private const int TamanhoMaximoDaPagina = 100;

    private readonly PetShopStore _store;

    public PetsV2Controller(PetShopStore store)
    {
        _store = store;
    }

    [HttpGet("pets", Name = "ListarPetsV2")]
    public IActionResult ListarPetsV2(
        [FromQuery] int page = 1,
        [FromQuery] int size = TamanhoPadraoDaPagina)
    {
        if (page < 1 || size < 1 || size > TamanhoMaximoDaPagina)
        {
            return Problem(
                title: "Parâmetro de paginação inválido.",
                detail: $"'page' começa em 1 e 'size' deve estar entre 1 e {TamanhoMaximoDaPagina}.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var todos = _store.Pets.OrderBy(p => p.Id).ToList();
        var itens = todos
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => new
            {
                id = p.Id,
                nomeDoPet = p.Nome,
                especie = p.Especie,
                raca = p.Raca,
                tutorId = p.TutorId
            })
            .ToList();

        return Ok(new
        {
            page,
            size,
            total = todos.Count,
            items = itens
        });
    }
}
