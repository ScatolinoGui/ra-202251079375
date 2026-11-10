# API Vazada - Operacao Blindagem

Projeto pratico da **Aula 07 - Seguranca em Aplicacoes Web (OWASP Top 10)** com as 4 vulnerabilidades corrigidas.

## As 4 correcoes aplicadas

| # | Falha original | Onde foi corrigida | Correcao |
|---|---|---|---|
| 1 | Injecao de SQL | `Controllers/ProdutosController.cs` -> `Buscar` | Parametrizacao com `FromSqlRaw("... LIKE {0}", $"%{nome}%")` |
| 2 | IDOR (Broken Access Control) | `Controllers/PedidosController.cs` -> `GetById` e `GetByUsuario` | Checagem `pedido.UsuarioId != UsuarioLogadoId` retornando `Forbid()` (403) |
| 3 | Segredos no codigo e erro exposto | `Program.cs` e `LoginController.cs` | Leitura via `IConfiguration` e `UseExceptionHandler("/erro")` fora de Development |
| 4 | Mass assignment | `Controllers/UsuariosController.cs` -> `Atualizar` | DTO `AtualizarUsuarioRequest(string Nome, string Email)` sem expor `Role` |

O detalhamento das analises encontra-se em `respostas.md`.
