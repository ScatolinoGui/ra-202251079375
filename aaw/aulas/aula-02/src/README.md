# BibliotecaApi - Aula 02 (REST Fundamentos)

Template de partida da prática: uma API REST de biblioteca com CRUD completo e
status codes corretos.

## Pré-requisitos

- .NET 8 SDK (ou superior)
- Postman ou cliente HTTP equivalente

## Como rodar

```bash
dotnet run
```

A API sobe em `http://localhost:5268` (Swagger UI em `/swagger`).

## Roteiro da prática

| Passo | Endpoint | Status esperados |
|-------|----------|------------------|
| 1 | `GET api/livros` | 200 |
| 2 | `GET api/livros/{id}` | 200 / 404 |
| 3 | `POST api/livros` | 201 + header `Location` / 400 sem campos obrigatórios |
| 4 | `PUT api/livros/{id}` | 200 / 404 |
| 5 | `DELETE api/livros/{id}` | 204 / 404 |

Implementação dos passos concluída em `Controllers/LivrosController.cs`, e respostas
teóricas do handout e cenários REST documentadas em `respostas.md`.
