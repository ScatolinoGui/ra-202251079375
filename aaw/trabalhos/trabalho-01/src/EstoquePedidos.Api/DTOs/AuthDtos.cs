namespace EstoquePedidos.Api.DTOs;

public record LoginRequest(
    string Email,
    string Senha);

public record RegistroRequest(
    string Nome,
    string Email,
    string Senha,
    string Role = "Usuario");

public record AuthResponse(
    string Token,
    string Nome,
    string Email,
    string Role);
