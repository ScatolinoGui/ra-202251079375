using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Models;

/// <summary>
/// Representa um autor de livros no sistema da biblioteca.
/// </summary>
public class Autor
{
    /// <summary>
    /// Identificador unico do autor.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Nome completo do autor.
    /// </summary>
    /// <example>Machado de Assis</example>
    [Required(ErrorMessage = "O nome do autor e obrigatorio.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 200 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Pais de origem do autor.
    /// </summary>
    /// <example>Brasil</example>
    [StringLength(100)]
    public string? Pais { get; set; }

    /// <summary>
    /// Breve biografia do autor.
    /// </summary>
    /// <example>Fundador da Academia Brasileira de Letras.</example>
    [StringLength(1000)]
    public string? Biografia { get; set; }

    /// <summary>
    /// Lista de livros associados a este autor.
    /// </summary>
    public List<Livro> Livros { get; set; } = new();
}
