# Aula 04 - REST na pratica: duas APIs para consumir

Duas APIs .NET 6 de mesmo porte e dominios diferentes. A primeira funciona como referencia de boas praticas REST; a segunda concentra o exercicio de auditoria e refatoracao dos 12 erros de design REST.

| Projeto | Tema | Porta | Papel |
|---|---|---|---|
| `CafeteriaApi/` | Cafe Newton (cafes, pedidos, itens) | **5301** | REST bem projetada (referencia) |
| `PetShopApi/` | PetHouse (pets, tutores, consultas) | **5302** | API auditada e refatorada (12 erros corrigidos) |
| `testes/` | - | - | colecoes Postman, arquivos `.http` e scripts curl |
| `respostas.md` | - | - | relatorio completo da auditoria dos 12 endpoints |

## Como rodar

Pre-requisito: **.NET 6 SDK** (ou superior, pois os projetos usam `RollForward` para rodar em .NET 8/9/10).

```bash
# terminal 1: API de referencia
cd "aaw/aulas/aula-04/src/CafeteriaApi"
dotnet run
# http://localhost:5301/swagger

# terminal 2: API PetHouse
cd "aaw/aulas/aula-04/src/PetShopApi"
dotnet run
# http://localhost:5302/swagger
```

As duas guardam os dados em memoria (`List<T>`): ao parar o processo, tudo volta ao estado inicial.

## Roteiro da pratica

1. **Aquecimento na API de referencia (`CafeteriaApi`).** Executar os 7 casos do guia: `200`, `201 + Location`, `400`, `404`, `204`, `404` na segunda exclusao e `304` com `If-None-Match`.
2. **Auditoria e refatoracao da `PetShopApi`.** Identificar os 12 erros de design REST, documentar o diagnostico em `respostas.md` e refatorar `PetShopController.cs`.

## Estrutura

```
src/
├── CafeteriaApi/          # versao de referencia (porta 5301)
│   ├── Controllers/       #   CafesController, PedidosController, PedidoItensController
│   ├── Data/              #   CafeteriaStore (240 cafes e 60 pedidos em memoria)
│   └── Models/
├── PetShopApi/            # versao refatorada (porta 5302)
│   ├── Controllers/       #   PetShopController (endpoints REST corrigidos)
│   ├── Data/              #   PetShopStore (2.000 pets, 6.000 consultas)
│   └── Models/
├── testes/                # Postman, .http e curl
└── respostas.md           # diagnostico detalhado dos 12 erros e solucoes
```
