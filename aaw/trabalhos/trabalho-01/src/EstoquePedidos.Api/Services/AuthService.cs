using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EstoquePedidos.Api.Config;
using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Exceptions;
using EstoquePedidos.Api.Models;
using EstoquePedidos.Api.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EstoquePedidos.Api.Services;

public class AuthService
{
    // Parametros do PBKDF2: salt de 128 bits, hash de 256 bits e 100 mil iteracoes
    private const int TamanhoSalt = 16;
    private const int TamanhoHash = 32;
    private const int Iteracoes = 100_000;
    private const int TamanhoMinimoSenha = 6;

    private static readonly string[] RolesPermitidas = { "Admin", "Usuario" };

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IOptions<JwtSettings> jwtSettings)
    {
        _usuarioRepository = usuarioRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse> RegistrarAsync(RegistroRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            throw new ArgumentException("O nome e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        {
            throw new ArgumentException("Informe um e-mail valido.");
        }

        if (string.IsNullOrWhiteSpace(request.Senha) || request.Senha.Length < TamanhoMinimoSenha)
        {
            throw new ArgumentException(
                $"A senha deve ter no minimo {TamanhoMinimoSenha} caracteres.");
        }

        var role = NormalizarRole(request.Role);
        var email = request.Email.Trim().ToLowerInvariant();

        var existente = await _usuarioRepository.ObterPorEmailAsync(email);
        if (existente is not null)
        {
            throw new ConflitoException("Ja existe um usuario cadastrado com este e-mail.");
        }

        var usuario = new Usuario
        {
            Nome = request.Nome.Trim(),
            Email = email,
            SenhaHash = GerarHashSenha(request.Senha),
            Role = role
        };

        var criado = await _usuarioRepository.CriarAsync(usuario);
        return CriarResposta(criado);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Mesma mensagem para e-mail inexistente e senha errada, para nao revelar quais contas existem
        const string mensagemFalha = "E-mail ou senha invalidos.";

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
        {
            throw new UnauthorizedAccessException(mensagemFalha);
        }

        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email.Trim());
        if (usuario is null || !VerificarSenha(request.Senha, usuario.SenhaHash))
        {
            throw new UnauthorizedAccessException(mensagemFalha);
        }

        return CriarResposta(usuario);
    }

    // Formato salvo no banco: "iteracoes.saltBase64.hashBase64"
    public static string GerarHashSenha(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            senha,
            salt,
            Iteracoes,
            HashAlgorithmName.SHA256,
            TamanhoHash);

        return $"{Iteracoes}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool VerificarSenha(string senha, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senhaHash))
        {
            return false;
        }

        var partes = senhaHash.Split('.');
        if (partes.Length != 3 || !int.TryParse(partes[0], out var iteracoes))
        {
            return false;
        }

        byte[] salt;
        byte[] hashEsperado;
        try
        {
            salt = Convert.FromBase64String(partes[1]);
            hashEsperado = Convert.FromBase64String(partes[2]);
        }
        catch (FormatException)
        {
            return false;
        }

        var hashCalculado = Rfc2898DeriveBytes.Pbkdf2(
            senha,
            salt,
            iteracoes,
            HashAlgorithmName.SHA256,
            hashEsperado.Length);

        // Comparacao em tempo constante evita ataque de temporizacao
        return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
    }

    private static string NormalizarRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return "Usuario";
        }

        var encontrada = RolesPermitidas.FirstOrDefault(
            r => r.Equals(role.Trim(), StringComparison.OrdinalIgnoreCase));

        return encontrada
            ?? throw new ArgumentException("Perfil invalido. Use \"Admin\" ou \"Usuario\".");
    }

    private AuthResponse CriarResposta(Usuario usuario)
    {
        return new AuthResponse(
            GerarToken(usuario),
            usuario.Nome,
            usuario.Email,
            usuario.Role);
    }

    private string GerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
