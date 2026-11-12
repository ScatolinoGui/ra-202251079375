using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Models;

/// <summary>
/// Representa o registro de emprestimo de um livro para um usuario.
/// </summary>
public class Emprestimo
{
    /// <summary>
    /// Identificador unico do emprestimo.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Identificador do livro emprestado.
    /// </summary>
    /// <example>1</example>
    [Required]
    public int LivroId { get; set; }

    /// <summary>
    /// Nome do usuario que realizou o emprestimo.
    /// </summary>
    /// <example>Joao Silva</example>
    [Required(ErrorMessage = "O nome do usuario e obrigatorio.")]
    [StringLength(200)]
    public string NomeUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora em que o emprestimo foi realizado.
    /// </summary>
    public DateTime DataEmprestimo { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data limite prevista para a devolucao do livro.
    /// </summary>
    public DateTime DataDevolucaoPrevista { get; set; } = DateTime.UtcNow.AddDays(14);

    /// <summary>
    /// Data em que o livro foi efetivamente devolvido (nulo se ainda ativo).
    /// </summary>
    public DateTime? DataDevolucaoEfetiva { get; set; }

    /// <summary>
    /// Status atual do emprestimo.
    /// </summary>
    /// <example>Ativo</example>
    public StatusEmprestimo Status { get; set; } = StatusEmprestimo.Ativo;
}

public enum StatusEmprestimo
{
    Ativo,
    Devolvido,
    Atrasado
}
