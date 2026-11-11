# Resolucao da atividade pratica - Aula 08 (Autenticacao e Autorizacao com JWT)

## Etapas implementadas na `BibliotecaApi`

1. **Etapa 1 (`Program.cs`)**: configuracao do esquema `JwtBearerDefaults.AuthenticationScheme` validando `Issuer`, `Audience`, `Lifetime` (`ClockSkew = TimeSpan.Zero`) e `IssuerSigningKey` (HMAC-SHA256), seguida do registro dos middlewares `app.UseAuthentication()` e `app.UseAuthorization()` na ordem exata.
2. **Etapa 2 (`AuthController.cs`)**: implementacao do endpoint `POST /api/auth/login`, que valida email e senha no `BibliotecaStore.Autenticar`, retorna `401 Unauthorized` para credenciais invalidas ou gera o token JWT assinado com as claims `NameIdentifier`, `Name`, `Email`, `Role` e `Jti`.
3. **Etapa 3 (`LivrosController.cs`)**: protecao das rotas de escrita `POST /api/livros` e `PUT /api/livros/{id}` com `[Authorize]`, mantendo `GET /api/livros` e `GET /api/livros/{id}` abertos para consulta publica.
4. **Etapa 4 (`LivrosController.cs`)**: restricao da rota `DELETE /api/livros/{id}` com `[Authorize(Roles = "Bibliotecario")]`, diferenciando `401 Unauthorized` (sem token ou token expirado) de `403 Forbidden` (autenticado como `Leitor`, mas sem o papel exigido).

## Resultado do ciclo de verificacao

| # | Requisicao | Status obtido | Motivo |
|---|---|---|---|
| 1 | `POST /api/livros` sem token | `401 Unauthorized` | Falta de cabecalho `Authorization: Bearer <token>` |
| 2 | `POST /api/auth/login` (`leitor@biblioteca.com`) | `200 OK` | Credenciais validas, retorna JWT com role `Leitor` |
| 3 | `POST /api/livros` com token de `Leitor` | `201 Created` | Usuario autenticado atende ao `[Authorize]` |
| 4 | `DELETE /api/livros/1` com token de `Leitor` | `403 Forbidden` | Usuario autenticado, mas sem a role `Bibliotecario` |
| 5 | `POST /api/auth/login` (`biblio@biblioteca.com`) | `200 OK` | Credenciais validas, retorna JWT com role `Bibliotecario` |
| 6 | `DELETE /api/livros/1` com token de `Bibliotecario` | `204 No Content` | Atende tanto a autenticacao quanto ao papel exigido |
| 7 | `POST /api/auth/login-expirado` + `POST /api/livros` | `401 Unauthorized` | `ValidateLifetime = true` e `ClockSkew = TimeSpan.Zero` rejeitam o token vencido |
| 8 | `GET /api/livros` sem token | `200 OK` | Endpoint publico sem `[Authorize]` |
| 9 | `GET /api/quem-sou-eu` com token | `200 OK` | Exibe as claims extraidas do payload do token |
