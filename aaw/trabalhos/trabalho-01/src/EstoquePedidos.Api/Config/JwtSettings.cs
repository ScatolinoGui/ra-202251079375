namespace EstoquePedidos.Api.Config;

// Lido da secao "JwtSettings"; em producao a chave vem de variavel de ambiente
// (ex.: JwtSettings__SecretKey) e nunca fica fixa no appsettings.json.
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 120;
}
