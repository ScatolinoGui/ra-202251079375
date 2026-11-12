using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Models;

/// <summary>
/// Representa um livro no acervo da biblioteca.
/// </summary>
public class Livro
{
    /// <summary>
    /// Identificador unico do livro.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Titulo da obra literaria.
    /// </summary>
    /// <example>Dom Casmurro</example>
    [Required(ErrorMessage = "O titulo e obrigatorio.")]
    [StringLength(300, MinimumLength = 1)]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Codigo internacional padronizado do livro (ISBN).
    /// </summary>
    /// <example>978-85-359-0277-7</example>
    [StringLength(20)]
    public string? Isbn { get; set; }

    /// <summary>
    /// Ano em que o livro foi publicado.
    /// </summary>
    /// <example>1899</example>
    [Range(1000, 2100, ErrorMessage = "Ano de publicacao invalido.")]
    public int AnoPublicacao { get; set; }

    /// <summary>
    /// Genero literario do livro.
    /// </summary>
    /// <example>Romance</example>
    [StringLength(100)]
    public string? Genero { get; set; }

    /// <summary>
    /// Identificador do autor do livro.
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "O autor e obrigatorio.")]
    public int AutorId { get; set; }

    /// <summary>
    /// Indica se o exemplar esta disponivel para novo emprestimo.
    /// </summary>
    /// <example>true</example>
    public bool Disponivel { get; set; } = true;
}
