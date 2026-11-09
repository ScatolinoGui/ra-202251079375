# PetShopApi - "PetHouse"

API do PetHouse utilizada na auditoria de design REST da Aula 04 e refatorada para corrigir os 12 problemas originais de modelagem de recursos, semantica HTTP, idempotencia, paginação, estado e cache.

## Rodar

```bash
dotnet run
```

`http://localhost:5302` (Swagger UI em `/swagger`).
Dados em memoria: 300 tutores, 2.000 pets, 6.000 consultas, recriados a cada execucao.

## Os 12 endpoints originais auditados

| # | Metodo original | Rota original | Rota refatorada |
|---|---|---|---|
| 01 | POST | `/api/v1/getPets` | `GET /api/v1/pets` |
| 02 | GET | `/api/v1/deletarPet?id=7` | `DELETE /api/v1/pets/{id}` |
| 03 | GET | `/api/v1/pet/{id}` | `GET /api/v1/pets/{id}` |
| 04 | GET | `/api/v1/banhosTosa` e `/api/v1/tutores_vip` | `GET /api/v1/banhos-e-tosas` e `GET /api/v1/tutores?vip=true` |
| 05 | POST | `/api/v1/pets` (retornando 200) | `POST /api/v1/pets` (`201 Created` + `Location`) |
| 06 | GET | `/api/v1/pets/{id}` (retornando 200 em inexistente) | `GET /api/v1/pets/{id}` (`404 Not Found` com `ProblemDetails`) |
| 07 | GET | `/api/pets` (quebra de contrato sem versao) | `GET /api/v1/pets` (estavel) e `GET /api/v2/pets` |
| 08 | GET | `/api/v1/petshops/1/clientes/5/pets/9/consultas/12/exames/6` | `GET /api/v1/exames/{id}` e `GET /api/v1/consultas/{id}/exames` |
| 09 | GET | `/api/v1/consultas` (6.000 itens sem paginação) | `GET /api/v1/consultas?page=1&size=20` |
| 10 | PUT | `/api/v1/pets/{id}/vacinas` (acumulando registros) | `POST /api/v1/pets/{id}/vacinas` e `PUT /api/v1/pets/{id}/vacinas` (idempotente) |
| 11 | POST/GET | `/api/v1/sessao` + `/api/v1/meus-pets` (sessao estatica) | `GET /api/v1/tutores/{tutorId}/pets` (stateless) |
| 12 | GET | `/api/v1/tabela-de-precos` (`no-store`) | `GET /api/v1/tabela-de-precos` (`Cache-Control` + `ETag` / `304`) |

O diagnostico completo de cada ponto encontra-se em `../respostas.md`.
