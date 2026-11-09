# Aula 05 - Persistencia em Arquiteturas Distribuidas - Pratica

Template da pratica de persistencias trocaveis com o padrao Repository (`IProdutoRepositorio`) e ferramenta visual de NoSQL.

## Conteudo desta pasta

| Item | O que e |
|------|---------|
| `nosql-explorer.html` | Ferramenta visual usada nas fases de exploracao e explicacao |
| `ServicoProdutos/` | Projeto .NET da pratica - API com `IProdutoRepositorio` e duas implementacoes (`Sql` e `Documento`) |
| `respostas.md` | Resolucao dos 4 cenarios de banco de dados, teorema CAP e comparativo pratico entre SQL e Documento |

## Como rodar

1. **Modo Sql**: `cd ServicoProdutos && dotnet run` (com `"Persistencia": "Sql"` em `appsettings.json`).
2. **Modo Documento**: alterar `"Persistencia"` para `"Documento"` em `appsettings.json` e executar `dotnet run`.
