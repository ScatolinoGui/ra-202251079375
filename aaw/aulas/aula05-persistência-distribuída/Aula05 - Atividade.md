# HANDOUT — AULA 05

## Escolha o Banco

*Persistência em arquiteturas distribuídas — Arquitetura de Aplicações Web*

## 🎯 MISSÃO

Vocês são o time de arquitetura de dados contratado pelas 4 empresas abaixo. Para CADA cenário:

- Escolham o modelo de banco: relacional, documento, chave-valor ou grafo
- Justifiquem com pelo menos 2 fatores do contexto (estrutura dos dados, padrão de acesso, escala, consistência...)
- Apontem o principal risco da escolha de vocês

*⏱️ Tempo: 25 minutos  |  👥 Formato: em duplas  |  Não existe resposta única — o que vale é a justificativa.*

> **Nomes:**  Randolfo A Gonçalves - 202451049275   **Turma:** GNP0547   **Data:** 03/08/2026
> **Nomes:** Guilherme P Scatolino - 202251079375   **Turma:** GNP0547   **Data:** 03/08/2026

## CENÁRIO 01 — TechStore — o catálogo camaleão

E-commerce com 80 mil produtos. Cada categoria tem atributos completamente diferentes: livro tem autor e número de páginas; notebook tem RAM e CPU; camiseta tem tamanho e cor.

- A cada categoria nova, o time faz ALTER TABLE e a tabela produtos já tem 92 colunas (a maioria NULL)
- O produto é quase sempre lido INTEIRO, de uma vez, para montar a página
- Novos atributos surgem toda semana — o marketing não espera o DBA
- Relatórios cruzando categorias são raros

**Sua análise:**

1. Modelo recomendado:   ☐ Relacional     ☐ Documento     ☐ Chave-valor     ☐ Grafo
>   Documento

2. Justificativa (mínimo 2 fatores do contexto):
>   - Flexibilidade para inserir novos produtos com estruturas dados distintas
>   - Maior facilidade para atualizar o estoque pois não implica numa atualização direta no banco de dados.

3. Principal risco da escolha:
>   - Dificuldade de consistência de dados principalmente no lançamento em estoque e apresentação em relatórios.

## CENÁRIO 02 — MegaCart — o carrinho da Black Friday

Serviço de carrinho de compras de um varejista gigante. Na Black Friday são milhões de leituras e escritas por minuto.

- O acesso é SEMPRE pela chave: “carrinho do cliente 12345” — nunca por busca ou filtro
- Todo carrinho expira automaticamente em 48h (TTL)
- Latência precisa ser de poucos milissegundos
- Perder um carrinho é chato, mas NÃO é tragédia — o cliente remonta

**Sua análise:**

1. Modelo recomendado:   ☐ Relacional     ☐ Documento     ☐ Chave-valor     ☐ Grafo
>   - Chave valor

2. Justificativa (mínimo 2 fatores do contexto):
>   - Velocidade de acesso da dados.
>   - Atualização rápida de estados do carrinho levando levando em consideração regras de contexto.

3. Principal risco da escolha:
>   - Dificuldade em estabelecer relações complexas conforme for o crescimento da aplicação.

## CENÁRIO 03 — PayBank — dinheiro não pode evaporar

Módulo de transferências de um banco. Uma transferência debita uma conta e credita outra — as duas operações têm que acontecer JUNTAS ou nenhuma acontece.

- Consistência forte exigida por lei — saldo errado é multa do Banco Central
- Auditoria cruza contas, clientes, agências e transações em relatórios complexos (joins)
- O esquema dos dados é estável há 10 anos
- Volume alto, mas previsível

**Sua análise:**

1. Modelo recomendado:   ☐ `Relacional`     ☐ `Documento`     ☐ `Chave-valor`     ☐ `Grafo`
>   - Relacional

2. Justificativa (mínimo 2 fatores do contexto):
>   - Por ser um modelo ACID ele garante a consistência e atomicidade das transações
>   - Suporte a um crescimento estruturado da aplicação.

3. Principal risco da escolha:
>   - Conforme a aplicação cresce maiores são os custos de manutenção e gestão de dados

## CENÁRIO 04 — FriendLink — amigos dos seus amigos

Rede social profissional em que o produto principal é a indicação: “pessoas que você talvez conheça” e “quem pode te apresentar à empresa X”.

- As consultas dominantes percorrem RELACIONAMENTOS: amigos dos amigos, caminhos de indicação com até 6 níveis
- Em banco relacional, cada nível vira um self-join — com 6 níveis a consulta já não responde
- Os dados de perfil são simples; o valor está nas CONEXÕES
- O grafo cresce milhões de arestas por dia

**Sua análise:**

1. Modelo recomendado:   ☐ Relacional     ☐ Documento     ☐ Chave-valor     ☐ Grafo
>   - Grafo

2. Justificativa (mínimo 2 fatores do contexto):
>   - Lidar com densas redes relacionais entre entidades no banco.
>   - Consultas mais simples ao banco de dados.

3. Principal risco da escolha:
>   -   Consumo de recursos principalmente energético.

## DESAFIO

1. Escolha um dos cenários e responda: se a rede particionar (metade dos servidores não enxerga a outra metade), o que o sistema deve fazer — parar de responder para não errar, ou continuar respondendo mesmo arriscando dados desatualizados?
>   - Escolhemos o modelo de Banco de Dados orientado a documento, pois ele garante que nossa aplicação continue respondendo o usuário,
>   mesmo que a consistência de dados   esteja desatualizado.

1.1 Qual letra do CAP vocês sacrificariam e por quê?
>   - Letra C, pois estamos sacrificando a consistência de dados em registrados.
