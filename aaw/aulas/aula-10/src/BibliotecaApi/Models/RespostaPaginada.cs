namespace BibliotecaApi.Models;

/// <summary>
/// Envelope de resposta paginada para listas de recursos.
/// </summary>
/// <typeparam name="T">Tipo do item contido na pagina.</typeparam>
public class RespostaPaginada<T>
{
    /// <summary>
    /// Lista de itens retornados na pagina atual.
    /// </summary>
    public List<T> Itens { get; set; } = new();

    /// <summary>
    /// Numero da pagina corrente (iniciando em 1).
    /// </summary>
    /// <example>1</example>
    public int Pagina { get; set; }

    /// <summary>
    /// Quantidade maxima de itens por pagina.
    /// </summary>
    /// <example>10</example>
    public int TamanhoPagina { get; set; }

    /// <summary>
    /// Quantidade total de registros disponiveis no acervo.
    /// </summary>
    /// <example>42</example>
    public int TotalItens { get; set; }

    /// <summary>
    /// Numero total de paginas calculado com base no total de itens e tamanho da pagina.
    /// </summary>
    /// <example>5</example>
    public int TotalPaginas => (int)Math.Ceiling((double)TotalItens / TamanhoPagina);
}
