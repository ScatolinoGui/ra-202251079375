# Resolucao da atividade pratica - Aula 05 (Persistencia e Padrao Repository)

## Parte 1: Handout - Escolha o banco

### Cenario 01 - TechStore (o catalogo camaleao)
1. **Modelo recomendado**: `[x] Documento` (ex.: MongoDB).
2. **Justificativa**:
   - **Esquema flexivel por agregado (*schema-on-read*)**: cada categoria (livro, notebook, camiseta) possui atributos proprios que mudam toda semana sem exigir `ALTER TABLE` nem gerar tabelas esparsas com dezenas de colunas `NULL`.
   - **Padrao de leitura por agregado completo**: o produto e carregado inteiro de uma so vez para renderizar a pagina de detalhes, evitando `JOINs` entre tabelas auxiliares de atributos.
3. **Principal risco da escolha**: duplicacao de dados desnormalizados e maior custo caso passem a exigir relatorios analiticos cruzando atributos de multiplas categorias sem indices secundarios adequados.

### Cenario 02 - MegaCart (o carrinho da Black Friday)
1. **Modelo recomendado**: `[x] Chave-valor` (ex.: Redis).
2. **Justificativa**:
   - **Acesso estritamente por chave primaria (`O(1)`) e latencia de milissegundos**: toda leitura e escrita ocorre pela chave `carrinho:cliente:12345`, sem buscas por campos internos.
   - **Expiracao nativa (TTL)**: bancos chave-valor em memoria suportam expiracao automatica por registro (48 horas) sem jobs extras de limpeza.
3. **Principal risco da escolha**: volatilidade em memoria caso ocorra falha antes do flush em disco e impossibilidade de realizar consultas ad-hoc por itens dentro dos carrinhos sem varrer todas as chaves.

### Cenario 03 - PayBank (dinheiro nao pode evaporar)
1. **Modelo recomendado**: `[x] Relacional` (ex.: PostgreSQL / SQL Server).
2. **Justificativa**:
   - **Transacoes ACID multi-registro**: debitar uma conta e creditar outra exige atomicidade, consistencia e isolamento fortes para impedir perda ou duplicacao de saldo.
   - **Esquema estavel e consultas de auditoria com JOINs**: a estrutura de contas, clientes, agencias e transacoes e fixa ha anos e demanda cruzamentos relacionais complexos para o Banco Central.
3. **Principal risco da escolha**: maior complexidade e custo para escalar escrita horizontalmente (*sharding* distribuido mantendo ACID) quando comparado a bancos NoSQL.

### Cenario 04 - FriendLink (amigos dos seus amigos)
1. **Modelo recomendado**: `[x] Grafo` (ex.: Neo4j).
2. **Justificativa**:
   - **Travessia eficiente de relacionamentos profundos**: consultas de ate 6 graus de separacao percorrem ponteiros diretos entre vertices e arestas em tempo proporcional ao subgrafo visitado, sem degradar como os `self-joins` recursivos do SQL.
   - **Modelagem centrada em conexoes**: o valor do negocio reside nas arestas de indicacao entre perfis e empresas.
3. **Principal risco da escolha**: menor eficiencia para agregacoes tabulares em lote (relatorios financeiros globais) e particionamento de grafos gigantes entre multiplos servidores.

### Desafio CAP
- **Cenario escolhido: Cenario 03 (PayBank)** -> Em caso de particao de rede (`P`), o sistema deve escolher **CP** (sacrificar Disponibilidade `A` para preservar Consistencia `C`), recusando ou bloqueando transferencias nas particoes isoladas para impedir gasto duplo ou saldo inconsistente.
- **Contraste com Cenario 02 (MegaCart)** -> No carrinho da Black Friday, escolhe-se **AP** (sacrificar Consistencia imediata `C` para manter Disponibilidade `A`), continuando a aceitar adicoes de itens mesmo com risco de divergencia temporaria.

---

## Parte 2: Comparacao pratica (`ProdutoRepositorioSql` vs `ProdutoRepositorioDocumento`)

### Passo 4: Onde roda o filtro de `GET /produtos/barato?max=100`?
- **No `ProdutoRepositorioSql` (EF Core + SQLite)**: a expressao `_db.Produtos.Where(p => p.Preco < precoMaximo).ToList()` e traduzida para SQL (`SELECT ... FROM Produtos WHERE Preco < @max`). O filtro roda **no motor do banco de dados**, trafegando para a aplicacao apenas os registros que atendem ao criterio.
- **No `ProdutoRepositorioDocumento` (arquivos JSON)**: como nao ha motor de banco nem indice secundario sobre `Preco`, o metodo `ObterAbaixoDe` precisa chamar `ObterTodos()` (lendo e desserializando todos os arquivos JSON do disco para a memoria da aplicacao) e aplicar o `.Where(...)` **na aplicacao**. Em colecoes gigantes, isso causaria *full scan* em disco e alto consumo de RAM; bancos de documentos reais como o MongoDB resolvem esse ponto mantendo indices B-Tree sobre os campos filtrados.

### Passo 5: Evolucao de esquema (descomentando `Tags` em `Produto.cs`)
- **No modo `Documento` (`Schema-on-read`)**: sobrevive sem migracao. Os arquivos `produto-*.json` antigos que nao possuem o campo `"Tags"` continuam sendo lidos normalmente pelo `JsonSerializer.Deserialize<Produto>`, que mantem o valor padrao `new List<string>()` para a propriedade ausente, enquanto novos documentos gravados ja incluem `"Tags"`.
- **No modo `Sql` (`Schema-on-write`)**: quebra sem migracao caso o arquivo `produtos.db` ja tenha sido criado anteriormente. Como a tabela `Produtos` no SQLite foi criada sem a coluna `Tags` e `EnsureCreated()` nao altera tabelas existentes, o EF Core lanca `SqliteException: SQLite Error 1: 'no such column: p.Tags'`. Para funcionar no modelo relacional, e obrigatorio aplicar uma *Migration* (`ALTER TABLE`) ou recriar o banco.
