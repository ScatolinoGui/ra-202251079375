# Respostas da Aula 03: Design de APIs REST (EscolaTech)

Resolução do handout "Consultoria de Design: a API da EscolaTech" e do mini-quiz da aula sobre nomenclatura de URIs, aninhamento, paginação, versionamento e `ProblemDetails` (RFC 9457).

## Parte 1: Auditoria dos 6 endpoints do handout

### Endpoint 01 (`POST /api/getAlunos`)
1. **Problemas identificados**:
   - Usa verbo HTTP `POST` para uma operação apenas de leitura (deveria ser `GET`, que é seguro e idempotente).
   - Coloca verbo na URI (`getAlunos`, padrão RPC em vez de substantivo no plural).
   - Devolve todos os 12.482 registros de uma só vez (~9 MB por chamada), sem paginação.
   - Falta versionamento explícito na rota (`/api/v1/`).
2. **Redesenho**:
   - `GET /api/v1/alunos?page=1&size=20` (opcionalmente `&sort=nome&curso=...`)
   - Resposta: `200 OK` com envelope paginado (`page`, `size`, `totalItens`, `totalPaginas`, `itens`) ou `400 Bad Request` (`ProblemDetails`) se `page`/`size` forem inválidos.

### Endpoint 02 (`GET /deletarAluno?id=7`)
1. **Problemas identificados**:
   - Viola a semântica de método seguro ao usar `GET` para apagar dados no servidor (links, pré-carregamento de navegador ou crawlers podem disparar exclusões acidentais).
   - Coloca verbo na URI (`deletarAluno`) e passa o identificador do recurso em query string (`?id=7`) em vez de segmento de caminho.
   - Retorna `200 OK` com texto puro `"OK"` mesmo quando o aluno não existe, mascarando falhas.
   - Sem versionamento (`/api/v1/`).
2. **Redesenho**:
   - `DELETE /api/v1/alunos/7`
   - Resposta: `204 No Content` quando remove com sucesso, ou `404 Not Found` (`ProblemDetails`) quando o aluno `7` não existe.

### Endpoint 03 (`POST /api/alunos` devolvendo `200 OK` + `"OK"`)
1. **Problemas identificados**:
   - Retorna `200 OK` com texto `"OK"` na criação em vez de `201 Created`.
   - Não devolve o cabeçalho `Location` nem a representação do aluno recém-criado com seu `id`, obrigando o cliente a baixar a lista inteira só para descobrir o ID gerado.
   - Falta versionamento (`/api/v1/`).
2. **Redesenho**:
   - `POST /api/v1/alunos`
   - Resposta: `201 Created` com header `Location: /api/v1/alunos/{id}` e JSON do aluno criado no corpo; `400 Bad Request` (`ProblemDetails`) se faltarem campos obrigatórios.

### Endpoint 04 (`GET /escolas/1/turmas/3/alunos/25/matriculas/88/disciplinas/12`)
1. **Problemas identificados**:
   - Aninhamento excessivo de 5 níveis acoplando a URL à hierarquia interna do banco e obrigando o app a conhecer 5 IDs antes da chamada.
   - Ausência do prefixo da API e versão (`/api/v1/`).
2. **Redesenho**:
   - Achatar para no máximo 2 níveis de recurso: `GET /api/v1/alunos/25/matriculas/88` (ou `GET /api/v1/matriculas/88`, já que `88` identifica a matrícula de forma direta) e `GET /api/v1/disciplinas/12`, passando critérios opcionais como escola, turma ou semestre via query string se necessário.
   - Resposta: `200 OK` ou `404 Not Found` (`ProblemDetails`).

### Endpoint 05 (`GET /api/alunos/7/matriculas` devolvendo HTML `200 OK`)
1. **Problemas identificados**:
   - Retorna status `200 OK` para uma condição de erro (recurso inexistente), escondendo a falha da camada HTTP.
   - Devolve HTML (`text/html`) em uma API JSON, quebrando o parser do app mobile.
   - Falta versionamento (`/api/v1/`).
2. **Redesenho**:
   - `GET /api/v1/alunos/7/matriculas`
   - Resposta: `200 OK` com o array JSON das matrículas quando o aluno existe, ou `404 Not Found` com `Content-Type: application/problem+json` no formato RFC 9457 (`type`, `title`, `status`, `detail`, `instance`).

### Endpoint 06 (`PUT /api/atualizarNotaParcial?aluno=7&disc=12&nota=8.5`)
1. **Problemas identificados**:
   - Verbo na URI (`atualizarNotaParcial`) e ausência de versionamento (`/api/v1/`).
   - Usa `PUT` (substituição integral) para atualizar apenas um campo parcial (nota), quando o método semântico é `PATCH`.
   - Envia todos os identificadores e o dado de alteração na query string sem corpo JSON.
   - Retorna `200 OK` com texto `"OK"` sem representação JSON nem tratamento de inexistência.
2. **Redesenho**:
   - `PATCH /api/v1/alunos/7/matriculas/12` (ou `PATCH /api/v1/matriculas/12`) com corpo JSON `{ "nota": 8.5 }`.
   - Resposta: `200 OK` com a matrícula atualizada (ou `204 No Content`), `400 Bad Request` se a nota estiver fora da faixa válida e `404 Not Found` (`ProblemDetails`) se o aluno ou a matrícula não existirem.

## Parte 2: Desafio de versionamento (não quebrar o app mobile antigo)

A decisão de design que faltava na API inteira é o **versionamento explícito no contrato**, como o prefixo na URI (`/api/v1/...`).

- Quando houver mudança que **quebra compatibilidade** (renomear campo esperado pelo app antigo, alterar tipo de dado ou remover campo), criamos `/api/v2/alunos` mantendo `/api/v1/alunos` no ar até o app mobile ser atualizado pelos usuários.
- Quando a mudança for apenas **aditiva** (incluir um novo campo opcional no JSON de resposta), não é necessário subir para `v2`, desde que o cliente antigo ignore campos desconhecidos.

## Parte 3: Mini-quiz da aula

1. **`GET /buscarPedidos` vs `GET /pedidos`**:
   O correto é `GET /pedidos`. A intenção de leitura já está expressa no verbo HTTP `GET`; repetir o verbo `"buscar"` na URI viola REST e mistura estilos RPC na rota.
2. **Rota completa com versão para a matrícula 88 do aluno 1024**:
   `GET /api/v1/alunos/1024/matriculas/88` (no máximo 2 níveis de aninhamento, substantivos no plural e prefixo `/api/v1`).
3. **Listagem devolvendo 50.000 registros de uma vez**:
   Aplicar **paginação no servidor** com parâmetros na query string (`?page=1&size=20` com teto máximo de segurança, devolvendo metadados `totalItens` e `itens`) e **projeção / filtros** via query string (como `?curso=...`) para retornar apenas os campos necessários de cada página, sem coleções filhas gigantes embutidas na listagem geral.
