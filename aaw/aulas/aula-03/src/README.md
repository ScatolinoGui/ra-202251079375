# EscolaApi ("API da EscolaTech") - Aula 03 (REST Design de APIs)

Refatoração completa da API da EscolaTech para corrigir os 6 anti-padrões de design analisados no handout.

## Pré-requisitos

- .NET 6 SDK (ou superior)
- Cliente HTTP (Swagger UI, Postman ou `.http`)

## Como rodar

```bash
dotnet run
```

A API sobe em `http://localhost:5214` (Swagger UI em `/swagger`).
O banco em memória é populado com 120 alunos e 240 matrículas.

## Correções aplicadas em `AlunosController.cs`

| # | Anti-padrão original | Correção aplicada |
| :---: | :--- | :--- |
| 1 | Verbo na URI (`getAlunos`, `deletarAluno` via POST) | Substantivos no plural (`/api/v1/alunos`) e verbos HTTP semânticos (`GET`, `POST`, `DELETE`) |
| 2 | Status codes errados (`200` para erro ou remoção) | Status semânticos (`200 OK`, `201 Created` + `Location`, `204 No Content`, `400`, `404`) |
| 3 | Aninhamento de 4 níveis na URL | Máximo de 2 níveis: `GET /api/v1/alunos/{id}/matriculas/{matriculaId}` com filtros via query string |
| 4 | Rotas sem versionamento | Prefixo explícito `/api/v1/` em todos os endpoints |
| 5 | Listagem completa sem paginação | Paginação com `?page=1&size=10`, ordenação `?sort=nome`, filtro `?curso=` e metadados (`totalItens`, `totalPaginas`) |
| 6 | Erro em HTML com status `500` | Padronização em `ProblemDetails` (`application/problem+json`) via `Problem(...)` |

As respostas do handout e do mini-quiz estão registradas em `respostas.md`.
