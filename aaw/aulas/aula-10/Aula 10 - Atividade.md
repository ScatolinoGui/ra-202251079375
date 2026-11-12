# HANDOUT - AULA 10

## Decifrando o Contrato

*Lendo uma especificacao OpenAPI - Arquitetura de Aplicacoes Web*

## MISSAO

Voces receberam o contrato OpenAPI de uma API que NUNCA viram - e nada mais: sem codigo, sem wiki, sem ninguem para perguntar. Provem que o contrato basta:

- Parte 1 - Interroguem a especificacao YAML do Card 1 e respondam as perguntas do Card 2
- Parte 2 - Comparem com a documentacao em prosa do Card 3 e registrem o que so a spec responde

*Tempo: 30 minutos | Formato: em duplas | So vale usar o que esta impresso neste handout.*

> **Nomes:** Guilherme / Pair Programming   **Turma:** AAW - Noite   **Data:** 11 / 11 / 2026

## CARD 01 - O contrato - especificacao OpenAPI (YAML)

Este e TODO o material que o time parceiro enviou. Leiam com calma - cada linha responde a uma pergunta do Card 2.

```yaml
openapi: 3.1.0
info:
  title: API Biblioteca
  version: 1.0.0
servers:
  - url: https://api.biblioteca.newton.br
paths:
  /api/v1/livros:
    get:
      summary: Lista os livros (paginado)
      parameters:
        - name: page
          in: query
          schema: { type: integer, default: 1 }
      responses:
        "200": { description: Lista de livros }
    post:
      summary: Cadastra um livro
      security:
        - bearerAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema: { $ref: "#/components/schemas/NovoLivro" }
      responses:
        "201": { description: Livro criado }
        "400": { description: Campos invalidos (ProblemDetails) }
        "401": { description: Token ausente ou invalido }
components:
  securitySchemes:
    bearerAuth:
      type: http
      scheme: bearer
      bearerFormat: JWT
  schemas:
    NovoLivro:
      type: object
      required: [titulo, autor]
      properties:
        titulo: { type: string, maxLength: 120 }
        autor:  { type: string }
        ano:    { type: integer, minimum: 1450 }
```

## CARD 02 - Interrogatorio do contrato

Respondam usando SOMENTE o Card 1:

1. Que endpoints/operacoes existem e o que cada um faz?
- GET /api/v1/livros: Lista livros com paginacao opcional via query param 'page' (default 1). Retorna 200 OK com lista de livros.
- POST /api/v1/livros: Cadastra um novo livro recebendo o schema NovoLivro no corpo da requisicao em application/json. Exige autenticacao bearerAuth e retorna 201 (criado), 400 (dados invalidos com ProblemDetails) ou 401 (nao autorizado).

2. Quais campos sao OBRIGATORIOS para cadastrar um livro? Que restricoes os campos tem?
- Obrigatorios: 'titulo' (string com tamanho maximo de 120 caracteres) e 'autor' (string).
- Opcional: 'ano' (inteiro com valor minimo de 1450, ano de invencao da imprensa moderna).

3. Em que situacoes a API devolve 400? E 401? Qual operacao exige token - e de que tipo?
- 400 Bad Request: Devolvido quando o corpo enviado no POST contem campos invalidos segundo o schema NovoLivro (titulo ausente ou com mais de 120 caracteres, autor ausente ou ano inferior a 1450).
- 401 Unauthorized: Devolvido quando a chamada a POST /api/v1/livros for realizada sem header Authorization ou com token ausente/invalido.
- Apenas a operacao POST /api/v1/livros exige token. O esquema de seguranca e do tipo HTTP Bearer com formato JWT (bearerAuth).

4. Montem a requisicao POST completa no papel (metodo, URL, headers, body JSON valido):
```http
POST https://api.biblioteca.newton.br/api/v1/livros HTTP/1.1
Host: api.biblioteca.newton.br
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.exemplo
Content-Type: application/json

{
  "titulo": "Dom Casmurro",
  "autor": "Machado de Assis",
  "ano": 1899
}
```

## CARD 03 - A mesma API, documentada "em prosa"

Versao que o parceiro tinha antes de adotar OpenAPI:

*"Para cadastrar um livro, envie os dados do livro para o endpoint de livros. E preciso estar autenticado. Os campos incluem titulo, autor e ano. Em caso de erro, a API retorna uma mensagem apropriada. Tambem e possivel listar os livros cadastrados."*

1. Listem 3 informacoes que voces usaram no Card 2 e que esta prosa NAO responde com precisao:
1) A definicao formal de obrigatoriedade e restricoes de schema (a prosa nao diz que titulo e autor sao obrigatorios nem indica o limite de 120 caracteres ou o ano minimo de 1450).
2) O mecanismo e local exato de autenticacao (a prosa so diz 'estar autenticado', omitindo que deve ser enviado no header Authorization com o prefixo Bearer e no formato JWT).
3) A URL exata com versionamento (/api/v1/livros) e o contrato de status HTTP com ProblemDetails no 400 e presenca do 401 em vez de erro generico.

## DESAFIO

1. Mesmo com a spec em maos, que pergunta a dupla AINDA precisaria fazer ao time da API?
- Como obter o token JWT de acesso: qual e o endpoint de login/emissao de credenciais (ex: POST /api/auth/login ou identity provider externo OAuth2/OIDC), quais credenciais sao necessarias e qual o tempo de expiracao do token, pois o contrato apenas declara o consumo do Bearer sem documentar o fluxo de autenticacao previo.
