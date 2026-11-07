# Controle de estoque e pedidos

Sistema web desenvolvido para a disciplina de Arquitetura de Aplicações Web (AAW). O objetivo da aplicação é gerenciar o catálogo de produtos em estoque e o registro de pedidos de venda, mantendo o relacionamento um-para-muitos (`1:N`) entre produtos e pedidos com validação de saldo disponível, cálculo automático de descontos e frete, controle de acesso por perfil e interface web assíncrona.

## Tecnologias utilizadas

- Backend: ASP.NET Core Web API (.NET 10) em C#
- Persistência: MongoDB 7 com `MongoDB.Driver`
- Segurança: autenticação JWT Bearer e autorização por papéis (`Admin` e `Usuario`)
- Documentação da API: OpenAPI / Swagger UI
- Testes automatizados: `xUnit`
- Frontend: HTML5, CSS3 e JavaScript puro com `fetch()`
- Infraestrutura local: Docker e Docker Compose

## Estrutura da solução

- `EstoquePedidos.sln`: arquivo de solução .NET na raiz do trabalho
- `src/EstoquePedidos.Api/`: API REST e arquivos estáticos da interface web
- `src/EstoquePedidos.Tests/`: suíte de testes unitários com `xUnit`
- `docker-compose.yml`: definição do container `mongo:7` com volume persistente na porta `27017`

## Como subir o banco e compilar localmente

1. Subir o serviço do MongoDB via Docker Compose:

```bash
docker compose up -d
```

2. Restaurar pacotes, compilar a solução e executar os testes:

```bash
dotnet build
dotnet test
```

3. Iniciar a API em ambiente de desenvolvimento:

```bash
dotnet run --project src/EstoquePedidos.Api
```
