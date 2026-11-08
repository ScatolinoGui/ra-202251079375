# Respostas da Aula 02: Protocolo HTTP e fundamentos REST

Resolução do handout "Dissecando o HTTP" (6 cards e tabela-síntese), refatoração de endpoints para o padrão RESTful e análise dos cenários de idempotência, atualização parcial e códigos de status.

## Parte 1: Análise das 6 requisições do handout

### Requisição 01 (`GET /api/livros` -> `200 OK`)
1. O cliente pediu a leitura completa da coleção de livros (`GET` no recurso `/api/livros`).
2. `200 OK` informa que a consulta deu certo e devolveu o array JSON dos livros no corpo da resposta.
3. Repetir 3 vezes não muda nada no servidor (`GET` é seguro e idempotente). A resposta retorna os mesmos livros enquanto ninguém criar, alterar ou apagar registros.

### Requisição 02 (`GET /api/livros/99` -> `404 Not Found`)
1. O cliente pediu os dados do livro específico com ID `99` (`GET /api/livros/99`).
2. `404 Not Found` informa que não deu certo por erro do cliente (família `4xx`): o recurso solicitado não existe no servidor.
3. Repetir 3 vezes não muda o estado do servidor. A resposta continua sendo `404 Not Found`.

### Requisição 03 (`POST /api/livros` -> `201 Created`)
1. O cliente pediu a criação de um novo recurso na coleção de livros enviando título e autor no JSON (`POST /api/livros`).
2. `201 Created` confirma que o novo livro foi criado com sucesso no servidor (família `2xx`).
3. Enviar esse `POST` 3 vezes cria **3 livros diferentes** no banco, cada um com um ID novo (`8`, `9`, `10`), porque `POST` **não é idempotente**. O cabeçalho `Location: /api/livros/8` indica a URI exata onde o recurso recém-criado pode ser consultado.

### Requisição 04 (`PUT /api/livros/7` -> `200 OK`)
1. O cliente pediu a substituição completa dos dados do livro `7` (`PUT /api/livros/7`).
2. `200 OK` confirma que o recurso foi atualizado e devolve a representação atualizada no corpo da resposta (também seria aceito `204 No Content` se não retornasse corpo).
3. Repetir 3 vezes com o mesmo JSON não muda o estado final do servidor além da primeira alteração (`PUT` é idempotente). O livro `7` continua com os mesmos dados e a resposta permanece `200 OK`.

### Requisição 05 (`DELETE /api/livros/7` -> `204 No Content`)
1. O cliente pediu a remoção do livro `7` (`DELETE /api/livros/7`).
2. `204 No Content` informa que deu certo e o item foi excluído, sem necessidade de retornar corpo JSON na resposta.
3. Repetindo o `DELETE`, o estado do servidor **não muda** após a primeira remoção (ele continua excluído), portanto `DELETE` é idempotente. Porém, a **resposta esperada na segunda chamada é `404 Not Found`**, pois o recurso de ID `7` já não existe mais para ser removido.

### Requisição 06 (`POST /api/livros` sem título -> `400 Bad Request`)
1. O cliente tentou cadastrar um livro enviando um JSON incompleto, apenas com `"autor"`.
2. `400 Bad Request` informa erro do cliente (`4xx`): os dados enviados violaram a validação de entrada (o campo obrigatório `Titulo` faltou).
3. Repetir 3 vezes não muda o estado do servidor (nenhum livro é gravado). A resposta continua sendo `400 Bad Request` detalhando o erro no campo `Titulo`.

### Tabela-síntese dos verbos HTTP

| Verbo | Para que serve | Seguro? | Idempotente? | Status típicos |
| :--- | :--- | :---: | :---: | :--- |
| `GET` | Consultar coleção ou recurso único | Sim | Sim | `200 OK`, `404 Not Found` |
| `POST` | Criar um novo recurso | Não | Não | `201 Created` (`Location`), `400 Bad Request`, `409 Conflict` |
| `PUT` | Substituir por completo um recurso existente | Não | Sim | `200 OK`, `204 No Content`, `400 Bad Request`, `404 Not Found` |
| `PATCH` | Atualizar parcialmente campos específicos | Não | Depende | `200 OK`, `204 No Content`, `400 Bad Request`, `404 Not Found` |
| `DELETE` | Remover um recurso por identificador | Não | Sim | `204 No Content`, `404 Not Found`, `409 Conflict` |

### Desafio do handout (`PATCH` vs `PUT`)
Para alterar apenas o apelido entre dezenas de campos do perfil de um usuário em um app de banco, o mais adequado é **`PATCH /api/usuarios/{id}`** enviando somente `{ "apelido": "NovoApelido" }`. Com `PUT`, o cliente precisaria enviar o objeto inteiro do perfil e correria o risco de apagar ou sobrescrever campos que não queria mexer.

## Parte 2: Refatoração de endpoints anti-REST para o padrão RESTful

Substantivos no plural identificam os recursos, a ação fica no verbo HTTP e filtros ou buscas vão na query string.

| # | Endpoint anti-REST | Problema identificado | Padrão RESTful corrigido | Status codes esperados |
| :-: | :--- | :--- | :--- | :--- |
| 01 | `GET /buscarUsuarios` | Verbo na URI (`buscar`) | `GET /api/usuarios` | `200 OK` |
| 02 | `POST /deletarProduto/42` | Verbo HTTP `POST` e ação na URI (`deletar`) | `DELETE /api/produtos/42` | `204 No Content`, `404 Not Found`, `409 Conflict` |
| 03 | `GET /pedidos/listarTodos` | Verbo na URI (`listarTodos`) | `GET /api/pedidos` | `200 OK`, `401 Unauthorized` |
| 04 | `POST /criarCliente` | Verbo na URI (`criar`) e singular/ação | `POST /api/clientes` | `201 Created` (`Location`), `400 Bad Request`, `409 Conflict` |
| 05 | `GET /produto/detalhes?id=10` | Singular na coleção e verbo/substantivo redundante (`detalhes`) | `GET /api/produtos/10` | `200 OK`, `404 Not Found` |
| 06 | `GET /produtos/filtrarPorCategoria/Livros` | Verbo na URI e filtro preso no caminho | `GET /api/produtos?categoria=Livros` | `200 OK`, `400 Bad Request` |
| 07 | `POST /atualizarEstoqueProduto/10` | Usa `POST` e verbo na URI para substituir/alterar campo | `PATCH /api/produtos/10` (ou `PUT /api/produtos/10`) | `200 OK`, `400 Bad Request`, `404 Not Found` |
| 08 | `GET /cancelarPedido/15` | `GET` alterando estado no servidor (viola método seguro) | `DELETE /api/pedidos/15` (ou `POST /api/pedidos/15/cancelamento`) | `204 No Content`, `404 Not Found`, `409 Conflict` |
| 09 | `GET /usuariosPorPedido/15` ou rota inversa para pedidos do cliente | Relação 1-N sem hierarquia de sub-recurso | `GET /api/clientes/15/pedidos` | `200 OK`, `404 Not Found` |
| 10 | `POST /loginUsuario` retornando `200 { erro: ... }` | Verbo na rota e erro mascarado em `200 OK` | `POST /api/auth/login` | `200 OK`, `400 Bad Request`, `401 Unauthorized` |

## Parte 3: Cenários de idempotência, `PUT` vs `PATCH` e status de segurança

1. **Idempotência no `GET` vs `POST` (`GET /api/livros/1` e `POST /api/livros` repetidos)**:
   `GET` apenas consulta a representação atual, sem efeitos colaterais no servidor (é seguro e idempotente). Já enviar o mesmo `POST /api/livros` 3 vezes seguidas cria 3 novos livros com IDs distintos no banco, alterando o estado da coleção a cada tentativa (não é seguro nem idempotente).
2. **Idempotência no `PUT` (`PUT /api/livros/7`)**:
   Quando o cliente envia o objeto completo para substituir o livro 7 três vezes seguidas, o estado final na coleção continua exatamente igual ao da primeira chamada (diferente de `POST`, não cria cópias novas). Por isso `PUT` não é seguro, mas é idempotente.
3. **Idempotência no `DELETE` (`DELETE /api/livros/7` duas vezes)**:
   A primeira execução remove o livro e retorna `204 No Content`. A segunda execução retorna `404 Not Found`, pois o livro não existe mais. Apesar de o código HTTP mudar, **o estado final no servidor é idêntico** (o livro 7 continua ausente), confirmando que `DELETE` é idempotente.
4. **Diferença prática entre `PUT` e `PATCH`**:
   `PUT` exige a representação inteira do recurso para substituí-lo por completo (por definição, sempre idempotente). `PATCH` aplica uma alteração parcial apenas nos campos informados no JSON. `PATCH` só é idempotente quando define um valor absoluto (ex.: `{ "disponivel": false }`), e deixa de ser idempotente quando descreve operação incremental (ex.: `{ "deltaEstoque": -1 }`).
5. **Diferença entre `401 Unauthorized` e `403 Forbidden`**:
   `401 Unauthorized` indica falha de **autenticação**: o servidor não sabe quem fez a requisição (token ausente, assinatura inválida ou token expirado). `403 Forbidden` indica falha de **autorização**: o cliente foi autenticado e seu token é válido, mas seu papel ou vínculo com o recurso não permite executar aquela operação (ex.: usuário com papel `Usuario` tentando apagar um recurso exclusivo de `Admin`).
6. **Statelessness e transparência de status HTTP (Mini-Quiz da aula)**:
   Devolver `200 OK` com `{ "erro": "livro não existe" }` quebra o contrato HTTP porque proxies, gateways e clientes tratam a chamada como sucesso; o correto é retornar `404 Not Found`. Em paralelo, guardar o carrinho de compras em variável estática ou memória RAM de um servidor viola o princípio *stateless* do REST, pois em um ambiente com balanceador de carga a próxima requisição pode cair em outra instância e perder os itens; o estado conversacional deve ficar no cliente (token/identificadores) ou persistido externamente no banco/cache compartilhado.
