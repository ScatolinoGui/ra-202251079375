# API Biblioteca - Operacao Cracha Nivel 2

Projeto pratico da **Aula 09 - RBAC, ABAC e PBAC (autorizacao)** com as 4 falhas corrigidas.

## Correcoes aplicadas

| # | Modelo | Arquivo corrigido | Solucao |
|---|---|---|---|
| 1 | RBAC | `Program.cs` + `LivrosController.cs` | Policies `GerenciarAcervo` (`Bibliotecario`, `Admin`) e `RemoverLivro` (`Admin`) |
| 2 | Negar por padrao + RBAC | `Program.cs` + `RelatoriosController.cs` | `FallbackPolicy` exigindo login e policy `VerRelatorios` (`Admin`) |
| 3 | ABAC | `Autorizacao/DonoDoEmprestimo.cs` | Comparacao `NameIdentifier == emprestimo.UsuarioId` (ou role `Bibliotecario`) |
| 4 | PBAC | `politicas.json` | Regra especifica `ObraRara` posicionada antes da regra geral `*` |

O detalhamento completo encontra-se em `respostas.md`.
