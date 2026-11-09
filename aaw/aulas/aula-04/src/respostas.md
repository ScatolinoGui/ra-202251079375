# Resolucao da atividade pratica - Aula 04 (REST na pratica: auditoria e refatoracao)

## Parte 1: Aquecimento na API de referencia (`CafeteriaApi` - porta 5301)

Antes de auditar a `PetShopApi`, foram executados os 7 casos de referencia na `CafeteriaApi` para validar o comportamento esperado de uma API REST:

1. **Listagem paginada e filtrada (`GET /api/v1/cafes?page=1&size=5&torra=media`)**: retorna `200 OK` com envelope contendo `page`, `size`, `total`, `totalDePaginas` e `items`, junto com os cabecalhos `ETag` e `Cache-Control: public, max-age=30`.
2. **Criacao de recurso (`POST /api/v1/cafes`)**: retorna `201 Created`, corpo com o recurso recem-criado (incluindo `id` gerado) e cabecalho `Location: http://localhost:5301/api/v1/cafes/{id}`.
3. **Erro de validacao (`POST /api/v1/cafes` com preco negativo ou nome vazio, ou `GET /api/v1/cafes?size=500`)**: retorna `400 Bad Request` no formato padronizado `application/problem+json` (RFC 7807 / RFC 9457) indicando o motivo exato da rejeicao.
4. **Recurso inexistente (`GET /api/v1/cafes/99999`)**: retorna `404 Not Found` com corpo `ProblemDetails` (`title`, `status`, `detail` e `instance`).
5. **Exclusao de recurso (`DELETE /api/v1/cafes/{id}`)**: retorna `204 No Content` sem corpo de resposta.
6. **Repeticao da exclusao (`DELETE /api/v1/cafes/{id}` pela segunda vez)**: retorna `404 Not Found`, comprovando que a idempotencia do `DELETE` diz respeito ao estado final do servidor (o recurso continua ausente), embora o codigo de status mude de `204` na primeira chamada para `404` nas seguintes.
7. **Cache condicional (`GET /api/v1/cafes/1` enviando `If-None-Match` com o valor do `ETag` recebido anteriormente)**: retorna `304 Not Modified` sem trafegar o corpo JSON pela rede.

---

## Parte 2: Tabela de auditoria dos 12 endpoints da `PetShopApi`

| # | O que eu chamei | O que a resposta mostrou | Regra REST violada | Como eu redesenharia |
|---|---|---|---|---|
| 01 | `POST /api/v1/getPets` | `200 OK` devolvendo uma lista com 2.000 pets; a acao (`get`) esta escrita no caminho e a leitura foi feita com `POST`. | Interface uniforme: URI deve ser substantivo que identifica o recurso e o verbo HTTP deve definir a operacao. | `GET /api/v1/pets?page=1&size=20&especie=&tutorId=` |
| 02 | `GET /api/v1/deletarPet?id=7` | `200 OK` com mensagem `"Pet 7 apagado com sucesso"`; abrir a URL no navegador remove o registro do banco. | Metodos seguros (*safe methods*): `GET` jamais pode causar efeito colateral ou apagar dados no servidor. | `DELETE /api/v1/pets/7` retornando `204 No Content` (ou `404 Not Found` se nao existir). |
| 03 | `GET /api/v1/pet/7` | `200 OK` com o pet 7, mas usando substantivo no singular enquanto outras rotas usam `/api/v1/pets`. | Consistencia de nomenclatura: colecoes e recursos devem usar substantivos no plural em toda a API. | `GET /api/v1/pets/7` |
| 04 | `GET /api/v1/banhosTosa` e `GET /api/v1/tutores_vip` | `200 OK`, mas misturando `camelCase` e `snake_case` nas URIs e transformando o filtro `vip` em nome de recurso. | Padronizacao de URIs em `kebab-case` e uso de query string para filtragem de colecoes. | `GET /api/v1/banhos-e-tosas` e `GET /api/v1/tutores?vip=true` |
| 05 | `POST /api/v1/pets` | Criou o pet mas retornou `200 OK` e nao enviou o cabecalho `Location`. | Semantica de criacao HTTP: `POST` que cria recurso deve retornar `201 Created` com cabecalho `Location`. | `POST /api/v1/pets` usando `CreatedAtAction(nameof(ObterPet), new { id = criado.Id }, criado)`. |
| 06 | `GET /api/v1/pets/99999` | Retornou status `200 OK` com JSON `{ "sucesso": false, "erro": "Pet nao encontrado" }`. | Semantica dos codigos de status HTTP (familia `4xx` para erros do cliente) e padronizacao `ProblemDetails`. | Retornar `404 Not Found` com `Problem(...)` (`application/problem+json`). |
| 07 | `GET /api/pets` | Rota sem prefixo de versao que renomeou o campo `nome` para `nomeDoPet`, quebrando clientes existentes. | Versionamento explicito de contrato: mudancas incompativeis (*breaking changes*) exigem nova versao (`/api/v2`). | Manter `GET /api/v1/pets` intacto com `nome` e publicar a quebra de contrato em `GET /api/v2/pets`. |
| 08 | `GET /api/v1/petshops/1/clientes/5/pets/9/consultas/12/exames/6` | URI com 5 niveis de aninhamento; o controller ignora os 4 primeiros IDs e busca apenas pelo `exameId`. | Hierarquia rasa de recursos (maximo de 1 a 2 niveis); recurso com identificador proprio deve ser *top-level*. | `GET /api/v1/exames/6` (busca direta) e `GET /api/v1/consultas/12/exames` (exames da consulta). |
| 09 | `GET /api/v1/consultas` | Retornou todas as 6.000 consultas de uma vez (~680 KB), sem paginação, sem filtro e sem limite de tamanho. | Paginacao defensiva, filtragem e ordenacao em colecoes volumosas. | `GET /api/v1/consultas?page=1&size=20&petId=&veterinario=&sort=` com teto `size <= 100` e envelope `Pagina<Consulta>`. |
| 10 | `PUT /api/v1/pets/12/vacinas` (3 chamadas iguais) | Cada chamada `PUT` adicionou uma nova vacina repetida (a lista subiu de 1 para 4 vacinas). | Idempotencia do metodo `PUT`: repetir a mesma requisicao `PUT` deve deixar o recurso no mesmo estado. | `POST /api/v1/pets/{id}/vacinas` para adicionar evento de vacina (`201 Created`) e `PUT /api/v1/pets/{id}/vacinas` para substituir a carteira inteira. |
| 11 | `POST /api/v1/sessao` seguido de `GET /api/v1/meus-pets` | O servidor guardou `_usuarioDaVez` em variavel `static`; quando outro usuario fez login, trocou a sessao do primeiro. | Restricao *Stateless* do REST: o servidor nao deve guardar estado de sessao do cliente em memoria entre requisicoes. | Remover variaveis estaticas e passar o contexto na requisicao: `GET /api/v1/tutores/{tutorId}/pets` (ou via token no `Authorization`). |
| 12 | `GET /api/v1/tabela-de-precos` | Dado que muda uma vez por ano devolvido com `Cache-Control: no-store, no-cache` e sem `ETag`. | Restricao *Cacheable* do REST: respostas de leitura estaveis devem declarar validade e permitir revalidacao condicional. | Enviar `Cache-Control: public, max-age=3600` e `ETag: "precos-v3"`, respondendo `304 Not Modified` quando `If-None-Match` coincidir. |

---

## Parte 3: Detalhamento das correcoes implementadas na `PetShopApi`

### 1. Verbo na URI e leitura com POST (`POST /api/v1/getPets` -> `GET /api/v1/pets`)
Na versao original, o cliente precisava enviar um `POST` para `/api/v1/getPets` apenas para consultar a lista de pets. Isso impede cache HTTP, quebra a semantica de leitura segura e coloca o verbo na URI. Na refatoracao, o endpoint foi unificado em `GET /api/v1/pets`, recebendo filtros opcionais (`especie`, `tutorId`) e parametros de paginação (`page`, `size`).

### 2. Efeito colateral em metodo seguro (`GET /api/v1/deletarPet` -> `DELETE /api/v1/pets/{id}`)
O metodo `GET` e definido pela RFC 9110 como seguro (*safe*), ou seja, pre-carregadores de navegadores, crawlers e proxies podem chama-lo sem esperar destruicao de dados. Excluir um registro via `GET` permite que ate uma tag `<img src=".../deletarPet?id=7">` apague dados. O endpoint foi substituido por `DELETE /api/v1/pets/{id:int}`, retornando `204 No Content` quando remove o pet ou `404 Not Found` (`ProblemDetails`) quando o ID nao existe.

### 3. Nomenclatura singular vs plural (`GET /api/v1/pet/{id}` -> `GET /api/v1/pets/{id}`)
Manter `/api/v1/pet/{id}` no singular ao lado de `/api/v1/pets` cria duas arvores de rotas diferentes para o mesmo recurso. O endpoint foi padronizado sob a colecao plural `GET /api/v1/pets/{id:int}`.

### 4. Inconsistencia de caixa e filtro embutido na rota (`/banhosTosa` e `/tutores_vip`)
A versao original misturava `camelCase` (`/api/v1/banhosTosa`) e `snake_case` (`/api/v1/tutores_vip`). O atributo `vip` tambem e uma propriedade booleana do recurso `Tutor`, nao um recurso separado. As rotas foram convertidas para `GET /api/v1/banhos-e-tosas` (em `kebab-case`) e `GET /api/v1/tutores?vip=true`, ambas paginadas.

### 5. Status code incorreto na criacao (`POST /api/v1/pets`)
Retornar `200 OK` ao criar um recurso omite a confirmacao semantica de criacao e nao informa onde o novo recurso pode ser consultado. O metodo `CadastrarPet` passou a retornar `CreatedAtAction(nameof(ObterPet), new { id = criado.Id }, criado)`, gerando status `201 Created` e o header `Location: /api/v1/pets/{id}`.

### 6. Erro disfarçado de sucesso (`200 OK` quando o pet nao existe)
Devolver `200 OK` com `{ "sucesso": false, "erro": "Pet nao encontrado" }` obriga todo cliente, gateway e ferramenta de observabilidade a abrir o corpo JSON para descobrir que a chamada falhou. A busca passou a retornar `404 Not Found` utilizando `Problem(...)` no padrao `application/problem+json` (RFC 7807 / RFC 9457).

### 7. Quebra de contrato sem versionamento (`GET /api/pets`)
O endpoint `/api/pets` havia sido criado sem versao na URI e renomeava a propriedade `nome` para `nomeDoPet`, quebrando clientes que dependiam do contrato da `v1`. Para resolver o problema respeitando a evolucao de contratos:
- `/api/v1/pets` foi mantido estavel com o campo `nome`;
- foi adicionado o `PetsV2Controller` sob `[Route("api/v2")]` expondo `GET /api/v2/pets` com o campo `nomeDoPet` para os novos consumidores.

### 8. Aninhamento profundo de 5 niveis (`/petshops/{id}/clientes/{id}/pets/{id}/consultas/{id}/exames/{id}`)
URIs com muitos niveis acoplam o cliente a toda a arvore de entidades e exigem IDs desnecessarios quando o recurso final ja possui identificador unico. A rota foi desmembrada em duas:
- `GET /api/v1/exames/{id:int}`: acesso direto ao recurso de primeiro nivel pelo seu ID;
- `GET /api/v1/consultas/{consultaId:int}/exames`: listagem dos exames pertencentes a uma consulta (1 nivel de aninhamento).

### 9. Colecao sem paginação (`GET /api/v1/consultas`)
Despejar 6.000 registros (~680 KB) em uma unica resposta degrada banco de dados, rede e memoria do cliente. Foi criada a classe generica `Pagina<T>` (`Page`, `Size`, `Total`, `TotalDePaginas`, `Items`) e aplicada paginação defensiva (`page >= 1` e `1 <= size <= 100`, retornando `400 Bad Request` via `ProblemDetails` fora desses limites), combinada com filtros por `petId`, `veterinario` e ordenacao por `sort=data` ou `sort=-data`.

### 10. `PUT` nao idempotente (`PUT /api/v1/pets/{id}/vacinas`)
O metodo `PUT` tem semantica de substituicao integral do estado do recurso; portanto, chamar `PUT` $N$ vezes com o mesmo payload deve resultar exatamente no mesmo estado de 1 chamada. Na versao original, cada `PUT` inseria uma nova dose duplicada na lista. A solucao separou as duas operacoes:
- `POST /api/v1/pets/{id:int}/vacinas`: adiciona uma nova aplicacao de vacina e retorna `201 Created`;
- `PUT /api/v1/pets/{id:int}/vacinas`: chama `_store.SubstituirCarteira(id, carteira)`, substituindo toda a carteira de vacinacao do pet de maneira idempotente (`200 OK`);
- `GET /api/v1/pets/{id:int}/vacinas`: consulta a carteira atual do pet.

### 11. Quebra da restricao Stateless (`POST /api/v1/sessao` + `GET /api/v1/meus-pets`)
Guardar `_usuarioDaVez` e `_tutorDaVez` em campos `static` no controller faz com que o estado de sessao fique na memoria do servidor e seja sobrescrito quando outro cliente faz login (causando vazamento de dados entre usuarios e impedindo escalabilidade horizontal). Os campos estaticos foram removidos e a consulta passou a receber o identificador explicitamente na requisicao via `GET /api/v1/tutores/{tutorId:int}/pets`.

### 12. Desperdicio de cache (`GET /api/v1/tabela-de-precos`)
A tabela de precos muda raramente, mas o endpoint original forçava `Cache-Control: no-store, no-cache`. Na versao corrigida:
- o `PetShopStore` expoe `VersaoDaTabelaDePrecos` (valor `3`), a partir do qual o controller gera o identificador `ETag: "precos-v3"`;
- se o cliente enviar `If-None-Match: "precos-v3"`, a API responde imediatamente `304 Not Modified` sem corpo;
- caso contrario, devolve `200 OK` com `ETag: "precos-v3"` e `Cache-Control: public, max-age=3600`.
