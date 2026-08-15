# Projeto do Semestre — Programação Web (v4)

**Curso:** ADS / SI / CC · **Carga horária:** 80h
**Formato:** individual · **Tema:** **definido pelo próprio aluno**, mediante proposta aprovada
**Entrega:** exclusivamente por repositório Git no GitHub · **Competição:** pontos de bônus no Demo Day

---

## 1. Conceito

O projeto é **individual** e é o fio condutor da disciplina: cada conteúdo novo é aplicado
imediatamente ao projeto do aluno, aula a aula.

**Cada aluno define o tema do seu próprio projeto.** O que é igual para todos não é o assunto,
mas o **piso técnico** (seção 4): o conjunto de requisitos que qualquer projeto precisa cumprir.
É esse piso que mantém a avaliação justa entre temas diferentes — o tema é o contexto, os
requisitos é que são avaliados.

O tema não é livre a ponto de ser aleatório: ele passa por uma **proposta aprovada pelo professor**
(seção 3), que verifica se o assunto escolhido comporta os requisitos das duas fases. Um tema mal
dimensionado é o principal motivo de projeto travado no meio do semestre — a aprovação existe para
evitar isso, não para restringir a criatividade.

---

## 2. Regras do tema

### 2.1 O que qualquer tema precisa comportar

O tema é livre, desde que o domínio escolhido permita, naturalmente, todos os itens abaixo:

| # | Exigência do domínio | Por quê |
|---|---|---|
| 1 | Uma **coleção de itens** com pelo menos **8 registros** e ao menos 4 atributos cada (ex.: produtos, vagas, eventos, receitas, filmes, trilhas, animais, laudos) | É o que sustenta listagem, cards, filtros e a renderização de listas em React |
| 2 | Uma **tela de detalhe** de um item da coleção | Sustenta a rota com parâmetro na Fase 2 |
| 3 | Pelo menos **um formulário** com 5+ campos e validação (cadastro, inscrição, contato, busca avançada) | Sustenta validação nativa (Fase 1) e formulário controlado (Fase 2) |
| 4 | Um **critério de filtro ou busca** sobre a coleção | Sustenta estado e interação |
| 5 | Dados obteníveis de **API pública, API própria ou mock local** (`json-server`, arquivo JSON servido) | Sustenta `fetch` + `useEffect` na Fase 2 |

> Se o tema não comporta os cinco itens, ele não é aprovado como está — a devolutiva do professor
> indica o ajuste de escopo necessário.

### 2.2 Temas vetados

- Reprodução de tutorial, curso ou vídeo passo a passo (o projeto seria do autor do tutorial, não do aluno).
- Projeto já entregue pelo aluno em outra disciplina, semestre ou instituição.
- Tema idêntico ao de outro aluno da mesma turma — vale a **ordem de proposta**; o segundo a propor
  recebe pedido de diferenciação. Domínios parecidos são aceitos desde que o recorte seja distinto.
- Conteúdo ilegal, discriminatório, de ódio, ou que exponha dados pessoais reais de terceiros.
- Escopo que dependa de back-end próprio complexo, autenticação real, pagamento real ou dados sigilosos.
  A disciplina é de **front-end**: back-end, quando existir, é mock ou API pública.

### 2.3 Proposta e aprovação

A proposta é um arquivo **`docs/proposta.md` commitado no próprio repositório** — não vale e-mail,
mensagem ou documento avulso.

| Etapa | Prazo (2026.2) | O que acontece |
|---|---|---|
| Apresentação das regras e criação do repositório | **07/08** (Aula 01) | Repositório criado, link registrado na planilha da turma |
| **Entrega da proposta** (`docs/proposta.md`) | **14/08, 23h59** (Aula 02) | Commit da proposta no repositório |
| Devolutiva e aprovação | **21/08** (Aula 03) | Aprovado · aprovado com ajustes · reprovado (reenviar em 48 h) |
| **Trava do tema** | **21/08** | A partir daqui o tema só muda com autorização escrita do professor |

**Sem proposta aprovada até 21/08**, o professor **atribui** ao aluno um tema do banco da seção 2.4.
Ninguém fica sem projeto — mas quem perde o prazo perde a escolha.

**Template da proposta** (copiar para `docs/proposta.md`):

```markdown
# Proposta de Projeto — Programação Web 2026.2

- **Aluno:** nome completo · **Curso/Turma:**
- **Repositório:** URL

## 1. Tema e problema
Duas a quatro frases: que problema o site resolve e para quem.

## 2. Público-alvo
Quem usa e em que situação.

## 3. Coleção de itens
Qual é a coleção (mín. 8 itens) e quais atributos cada item tem.
Exemplo de um item preenchido.

## 4. Telas previstas
Lista das telas (mín. 3): o que cada uma mostra.

## 5. Formulário
Qual formulário existe, quais campos e que validações terá.

## 6. Filtro/busca
Por qual critério o usuário filtra ou busca a coleção.

## 7. Origem dos dados na Fase 2
API pública (qual, com link), API própria ou mock local (json-server / JSON estático).

## 8. Diferencial pretendido
O que este projeto terá além do piso obrigatório.
```

### 2.4 Banco de temas (para quem não quiser propor, ou perder o prazo)

| # | Tema | Recorte sugerido |
|---|------|------------------|
| 1 | ONGs locais + voluntários | Listagem por causa, inscrição de voluntário, painel |
| 2 | Produtividade universitária | Trilhas de estudo, agenda de entregas, filtros por disciplina |
| 3 | Vitrine de negócios/produtores locais | Cards de produto, carrinho, filtros por categoria |
| 4 | Agricultura local | Produtores, sazonalidade, cestas, mapa |
| 5 | Pets locais para adoção | Filtros por porte/idade/espécie, formulário de adoção |

---

## 3. Regras de entrega

1. **A entrega é o repositório Git no GitHub. Só isso.**
   O que é corrigido é o **estado do repositório na data e hora limite**. Não existe entrega por
   `.zip`, e-mail, Google Drive, OneDrive, WhatsApp, Teams, pendrive, print de tela ou upload na
   plataforma. Material enviado por esses meios **não é recebido nem corrigido** (ver D1).
2. **O link do repositório é registrado uma única vez**, na planilha/formulário da turma, até
   **21/08**. Trocar de repositório depois disso exige aviso ao professor **antes** do prazo da fase.
3. **O professor precisa ter acesso.** Repositório público é o recomendado. Se for privado, o
   aluno deve convidar o usuário do professor como colaborador e **confirmar que o convite foi
   aceito** — convite pendente na hora do prazo conta como repositório inacessível (D2).
4. **`README.md` na raiz**, contendo: nome completo, curso e turma, descrição do projeto em um
   parágrafo, pré-requisitos, comandos de instalação e execução, uma seção **“Uso de IA”**
   (seção 6.3) e — na Fase 2 — a **URL pública** da aplicação publicada. Critério prático: um
   terceiro roda o projeto seguindo só o README.
5. **Histórico de commits incremental.** O projeto é construído ao longo do semestre e o histórico
   do Git é a prova disso. Não é recomendação: é requisito eliminatório (D3).
6. **Prazos** (horário oficial: 23h59 de Brasília, carimbo do commit no GitHub):

| Entrega | Prazo | Vale |
|---|---|---|
| Proposta de tema | 14/08/2026 | condição para escolher o tema |
| **Fase 1** — site responsivo | **18/09/2026** | 10 pts da AV1 |
| **Fase 2** — aplicação React publicada | **25/11/2026** | 20 pts da AV2 |
| Demo Day — pitch de 5 min + arguição | 27/11/2026 | bônus de +3 a +5 pts |

> Commits posteriores ao prazo **são ignorados na correção**: o professor avalia o repositório no
> estado do último commit anterior ao limite (`git log --before="2026-09-18 23:59"`).

---

## 4. Piso técnico obrigatório

Igual para todos, independentemente do tema. É a base da rubrica (seção 8).

### Fase 1 — Site responsivo (entrega 18/09 · 10 pts)

- [ ] Mínimo de **3 páginas HTML** interligadas por navegação (ex.: home, listagem, detalhe/sobre)
- [ ] **HTML5 semântico**, com **0 erros** no validador do W3C
- [ ] Listagem da coleção com **≥ 8 itens** em cards
- [ ] **1 formulário** com 5+ campos, labels associados e validação nativa (`required`, `type`, `pattern`)
- [ ] CSS próprio com **custom properties** usadas como design tokens (cores, tipografia, espaçamento)
- [ ] **Flexbox e Grid**, ambos usados, cada um onde faz sentido
- [ ] **Mobile-first** com pelo menos 2 breakpoints; sem scroll horizontal a partir de 320 px
- [ ] **Lighthouse: Acessibilidade ≥ 90** (contraste, foco visível, textos alternativos, navegação por teclado)
- [ ] `README.md` completo e repositório organizado (sem lixo versionado)

### Fase 2 — Aplicação React publicada (entrega 25/11 · 20 pts)

- [ ] Projeto **Vite + React 19**, rodando com `npm install && npm run dev`
- [ ] **≥ 5 componentes próprios**, com props e composição
- [ ] Estado local com **`useState`** e **≥ 1 formulário controlado**
- [ ] **`useEffect` + `fetch`** consumindo API (pública, própria ou mock), tratando **loading e erro**
- [ ] **React Router** com **≥ 3 rotas**, incluindo rota de **detalhe com parâmetro**
- [ ] **≥ 1 Context** (ou estado compartilhado equivalente) justificado no README
- [ ] **≥ 1 teste de componente** com Vitest + Testing Library, passando em `npm test`
- [ ] **Deploy público funcionando** (Vercel, Netlify ou GitHub Pages), com a URL no README
- [ ] Responsividade e acessibilidade da Fase 1 preservadas

---

## 5. Desclassificação (nota **zero** na fase)

As situações abaixo **zeram a fase**, mesmo que o resultado final funcione bem. Não há correção
parcial nesses casos — a entrega sequer é avaliada. As regras são objetivas de propósito: dá para
o aluno conferir sozinho, antes de entregar, se está em alguma delas (seção 7).

### D1 · Entrega fora do Git/GitHub
Qualquer tentativa de entregar por `.zip`, e-mail, Drive, OneDrive, WhatsApp, Teams, pendrive,
print, PDF ou upload na plataforma. **Não é recebida.** Se, no prazo, não existir repositório
Git com o projeto, a fase é zero.

### D2 · Repositório inacessível ao professor no prazo
Repositório privado sem convite aceito, URL errada ou desatualizada na planilha, repositório
apagado, renomeado ou tornado privado sem aviso prévio. **O ônus de garantir o acesso é do aluno**;
a correção não é reagendada por isso.

### D3 · *Dump commit* — histórico não incremental
O projeto tem de ter sido **construído ao longo do semestre**. Caracteriza *dump commit*, e zera a
fase, **qualquer uma** das situações abaixo:

| Regra | Fase 1 | Fase 2 |
|---|:---:|:---:|
| Número mínimo de commits na fase | **≥ 8** | **≥ 10** |
| Dias-calendário distintos com commit | **≥ 5** | **≥ 6** |
| Semanas distintas com commit | **≥ 4** | **≥ 4** |
| Proporção máxima do código da fase introduzida por um único commit | **< 60 %** | **< 60 %** |
| Proporção máxima de commits feitos nas últimas 48 h antes do prazo | **≤ 50 %** | **≤ 50 %** |

Também caracteriza *dump commit*:

- Projeto inteiro aparecendo pronto de uma vez, ainda que fatiado em vários commits no mesmo dia.
- Histórico criado por **upload em bloco pela interface web** do GitHub (commits do tipo
  “Add files via upload” contendo o projeto inteiro).
- Repositório criado depois do prazo da proposta e “preenchido” retroativamente.

### D4 · Histórico forjado
Adulteração de datas para simular incrementalidade: `--date` manipulado, `commit --amend` ou
`rebase` em massa para redistribuir datas, commits cuja *author date* é incoerente com a data de
push/registro no GitHub. É mais grave que o D3: zera a fase e é tratado como fraude acadêmica.

### D5 · Autoria
Plágio, cópia de projeto de colega ou de terceiros, fork/clone de repositório alheio apresentado
como próprio, projeto encomendado, e **incapacidade de explicar o próprio código em arguição**
(seção 6.1). Em caso de dois projetos substancialmente iguais, **ambos** são zerados, salvo prova de
autoria de um deles.

### D6 · Entrega vazia ou fora do escopo aprovado
Repositório com apenas o template do Vite ou o `index.html` inicial; projeto diferente do aprovado
na proposta, sem autorização escrita; projeto reaproveitado de outra disciplina/semestre; código
que não abre nem roda por não estar de fato implementado.

> **Consequência comum a D1–D6:** nota **0** na fase. Como a Fase 1 vale 10 pts da AV1 e a Fase 2
> vale 20 pts da AV2, o aluno não fica automaticamente reprovado — mas perde integralmente a
> pontuação da fase, sem substituição, sem entrega alternativa e sem reabertura de prazo.

---

## 6. Penalidades (descontam, não zeram)

| # | Situação | Desconto |
|---|---|---|
| P1 | **Atraso** (último commit após o prazo) | −20 % da nota da fase por dia corrido, até 72 h; **após 72 h, nota 0** |
| P2 | `README.md` ausente ou que não permite rodar o projeto | −20 % da nota da fase |
| P3 | Mensagens de commit não descritivas (`update`, `aaa`, `.`, `teste`) em **mais de 50 %** dos commits | −10 % da nota da fase |
| P4 | Lixo versionado: `node_modules/`, `dist/` desnecessário, arquivos de IDE, `.env` com segredos | −10 % da nota da fase (segredo exposto deve ser revogado imediatamente) |
| P5 | Item do piso técnico (seção 4) ausente ou incompleto | perde os pontos do critério correspondente na rubrica |

## 6.1 Arguição técnica

A arguição é o mecanismo que valida a **autoria real** do projeto. Ela vale para **todos os
alunos**, sem exceção e sem seleção prévia — não é acusação nem sinal de suspeita, é parte do
protocolo de avaliação, anunciada na Aula 01.

**Regra central:** a pergunta é sempre sobre o **código do próprio aluno**, aberto na tela, nunca
sobre teoria decorada. O que se verifica não é se o aluno usou IA, mas se ele **entende o que
entregou** — quem escreveu e quem revisou linha a linha respondem igualmente bem; quem colou sem
ler, não.

### Quando acontece

| Momento | Formato | Peso |
|---|---|---|
| **Devolutiva da Fase 1** (25/09, após a AV1) | 2 min por aluno, individual, na mesa do professor | **Formativa** — não altera nota; serve de ensaio e de aviso |
| **Demo Day** (27/11) | **1 min ao final do próprio pitch**, diante da turma (seção 10) | **Valendo** — define o nível A/B/C abaixo |
| **A qualquer momento** | O professor pode pedir explicação de um commit ou trecho durante as aulas práticas | Formativa |

### Como o trecho é sorteado

No último minuto do pitch, o professor escolhe **na hora**, a partir do que está na tela ou no
editor do aluno, um destes alvos:

1. um **componente** do projeto (Fase 2) ou um bloco de CSS/HTML (Fase 1);
2. um **commit** do meio do semestre, aberto no GitHub;
3. uma **função com mais de 10 linhas**.

> Por isso o aluno chega ao Demo Day com **o projeto rodando localmente e o editor aberto** — é
> requisito do pitch, não improviso (ver seção 10).

### Tipos de pergunta

| Tipo | Objetivo | Exemplos |
|---|---|---|
| **A · Leitura** | O aluno sabe ler o que entregou? | “Explique o que este trecho faz.” · “Este componente recebe estas props — quem as passa, e de onde vêm os dados?” · “O que aparece na tela se este array vier vazio?” |
| **B · Decisão** | Houve escolha, ou foi aceite automático? | “Por que este estado está aqui e não no componente pai?” · “Por que este `useEffect` tem esse array de dependências? O que muda se ficar vazio?” · “Por que Grid aqui e Flexbox ali?” · “Este Context poderia ser props — por que preferiu Context?” |
| **C · Modificação** | A mais discriminante: quem não escreveu não sabe onde mexer. | “Se eu quiser acrescentar um campo ao card, quais arquivos você mexe e em que ordem?” · “Onde você mudaria para o filtro aceitar dois critérios ao mesmo tempo?” · “Como fazer a lista mostrar uma mensagem quando a busca não retorna nada?” |
| **D · Falha** | O aluno pensou no que dá errado? | “Se a API cair agora, o que o usuário vê? Mostre no código onde isso é decidido.” · “O que quebra se eu apagar esta linha?” |

> Uma pergunta por aluno, do tipo adequado ao trecho aberto. Em caso de resposta insuficiente, o
> professor faz **uma segunda pergunta, mais simples**, sobre outro trecho — ninguém é reprovado
> por travar numa pergunta só.

### Níveis de resposta

| Nível | O que caracteriza | Efeito na nota da fase |
|:---:|---|---|
| **A** | Explica o próprio código, justifica a decisão e sabe o que quebra se mudar | Nota integral |
| **B** | Reconhece o código e explica **o quê**, mas não **o porquê**; hesita na modificação | **−20 %** da nota da fase |
| **C** | Não reconhece o próprio código, não sabe onde ele está, ou atribui a autoria a outra pessoa/ferramenta | Encaminha para **arguição individual** (abaixo) |

### Devido processo no nível C

O nível C **não zera ninguém no palco**. O aluno é chamado para uma **arguição individual de
10 minutos**, agendada em até 5 dias úteis, sem plateia, com três perguntas sobre partes
diferentes do projeto. Só a partir dela o professor decide:

- respondeu na arguição individual → nível A ou B, com a nota correspondente;
- não respondeu → **D5**, nota **0** na fase, com registro escrito do que foi perguntado e do que
  foi respondido.

Nervosismo, travamento e português truncado **não** caracterizam nível C. O que caracteriza é não
saber **onde** o código está nem **o que** ele faz.

## 6.2 Uso de IA

Ferramentas de IA são **permitidas** como apoio — é assim que o mercado trabalha, e proibi-las
seria treinar para um mercado que não existe. A disciplina cobra duas coisas, e só essas:

1. **Entender e saber explicar tudo o que entregou** (verificado na arguição, seção 6.1).
2. **Histórico que comprove construção incremental** (verificado no `git log`, regra D3).

Colar um projeto inteiro gerado de uma vez falha nas duas: cai em **D3** pelo histórico e em **D5**
na arguição. Usar IA para escrever uma função, entender o que ela faz e commitar junto com o resto
do avanço da aula não fere nenhuma regra.

## 6.3 Declaração de uso de IA no README

Obrigatória em ambas as fases, em uma seção `## Uso de IA` no `README.md`, com 3 a 6 linhas:

```markdown
## Uso de IA

- Ferramentas usadas: (ex.: ChatGPT, Copilot, Claude — ou "nenhuma")
- Onde ajudou: (ex.: gerou o esqueleto do formulário controlado e explicou o
  array de dependências do useEffect da tela de listagem)
- O que eu revisei/reescrevi: (ex.: refiz o tratamento de erro, que vinha
  genérico demais, e troquei o filtro por um que aceita busca vazia)
```

**Declarar não penaliza.** A declaração existe para tornar a conversa honesta e para o próprio
aluno mapear onde ele está mais frágil — que é exatamente onde a pergunta da arguição costuma cair.
Omitir a seção conta como README incompleto (**P2**). Declarar não substitui entender: a arguição
acontece do mesmo jeito.

---

## 7. Autoverificação antes de entregar

Rode no seu repositório, **antes** do prazo. Se algum número ficar abaixo do exigido na seção D3,
ainda dá tempo de corrigir o hábito — não dá para corrigir depois.

```bash
# quantos commits no total
git rev-list --count HEAD

# lista de commits com data e mensagem (confira se as mensagens descrevem o que mudou)
git log --date=short --pretty=format:"%h %ad %s"

# em quantos DIAS distintos você commitou
git log --date=short --pretty=%ad | sort -u | wc -l

# em quantas SEMANAS distintas você commitou
git log --date=format:"%G-W%V" --pretty=%ad | sort -u | wc -l

# tamanho de cada commit (procure um commit gigante isolado = sinal de dump)
git log --shortstat --date=short --pretty=format:"%h %ad %s"

# como o professor vai ver a sua entrega no prazo da Fase 1
git log --before="2026-09-18 23:59" -1
```

No PowerShell, troque `sort -u | wc -l` por `Sort-Object -Unique | Measure-Object`.

**Checklist final:**

- [ ] O link na planilha da turma aponta para este repositório
- [ ] O professor consegue abrir o repositório (testei numa aba anônima, ou o convite foi aceito)
- [ ] `README.md` permite que um terceiro rode o projeto
- [ ] Os números do `git log` atendem à tabela do D3
- [ ] `node_modules/` está no `.gitignore` e **não** está versionado
- [ ] (Fase 2) A URL publicada abre e funciona
- [ ] Fiz `git push` — o que está só na minha máquina **não foi entregue**

---

## 8. Rubrica

### Fase 1 — 10 pontos

| Critério | Pontos |
|---|:---:|
| Piso técnico: 3+ páginas, HTML semântico válido no W3C, coleção com 8+ itens, formulário com validação | 3,0 |
| Layout e responsividade: Flexbox + Grid, mobile-first, 2+ breakpoints, sem quebra a 320 px | 2,5 |
| CSS e consistência visual: custom properties como design tokens, hierarquia tipográfica, UX | 1,5 |
| Acessibilidade: Lighthouse ≥ 90, contraste, foco visível, alt, navegação por teclado | 1,5 |
| README e organização do repositório | 1,0 |
| Qualidade do histórico de commits (acima do mínimo do D3) | 0,5 |
| **Total** | **10,0** |

### Fase 2 — 20 pontos

| Critério | Pontos |
|---|:---:|
| Componentização: 5+ componentes próprios, props e composição | 3,0 |
| Estado: `useState` e formulário controlado | 3,0 |
| Efeitos e API: `useEffect` + `fetch`, com tratamento de loading e erro | 4,0 |
| Navegação e estado compartilhado: React Router (3+ rotas, detalhe com parâmetro) e Context | 3,0 |
| Teste automatizado com Vitest + Testing Library | 2,0 |
| Deploy público funcionando e documentado no README | 2,0 |
| UX/design e responsividade preservadas | 2,0 |
| Qualidade do histórico de commits (acima do mínimo do D3) | 1,0 |
| **Total** | **20,0** |

> A **arguição técnica** do Demo Day (seção 6.1) não soma pontos: ela **valida** os que foram
> atribuídos. Nível A mantém a nota, nível B desconta 20 % do total da fase e nível C confirmado
> em arguição individual cai em D5 (nota 0).

---

## 9. Exceções e recursos

- **Imprevistos** (saúde, trabalho, luto, problema técnico grave) devem ser comunicados ao professor
  **antes do prazo**, com a devida comprovação. Comunicado depois do prazo não reabre entrega.
- **Problema de infraestrutura** (GitHub fora do ar, apagão) deve ser reportado **no dia**, com
  evidência. Falha de máquina pessoal não é justificativa: o Git é distribuído e o `push` pode ser
  feito de qualquer computador do laboratório.
- Casos não previstos aqui são decididos pelo professor, ouvida a coordenação quando cabível.

---

## 10. Demo Day — apresentação, arguição e competição (27/11)

Nos dois últimos tempos de 27/11, cada aluno tem um **slot cronometrado** com duas partes:

| Parte | Tempo | O que acontece |
|---|:---:|---|
| **Pitch** | 5 min | O aluno abre dizendo, em uma frase, **que problema o projeto resolve** (os temas são distintos — ninguém conhece o contexto do outro), demonstra a aplicação ao vivo, explica **uma decisão técnica** que tomou e fecha com o próximo passo |
| **Arguição técnica** | 1 min | O professor escolhe um trecho do código na tela e faz **uma pergunta** sobre ele (seção 6.1). O aluno responde no código, não de memória |
| Troca | 30 s | Próximo aluno conecta e abre o projeto |

**Slot de 6min30 por aluno.** É um pitch de conferência, não relâmpago: cabe demonstrar um fluxo
completo do usuário com calma e sustentar a decisão técnica com alternativa e custo.

### Quantos cabem

Os dois últimos tempos de 27/11 somam 100 min: cerca de **60 min de rodada**, mais abertura (7 min)
e apuração, premiação e encerramento (30 min).

| Turma | Cabe? | Como |
|:---:|:---:|---|
| **até 9 alunos** | ✅ | Todos apresentam, rodada de ~60 min |
| **10 a 11** | ⚠️ | Todos apresentam, cortando a abertura para 3 min e a premiação para 25 min (rodada de 70 min) |
| **12 ou mais** | ❌ | 12 alunos × 6min30 = 78 min só de rodada. Não cabe — escolher uma das saídas abaixo |

**Saídas para turma acima de 11 alunos** (decidir com a turma na aula 19, nunca no dia):

1. **Finalistas (recomendada com pitch de 5 min).** Votação da turma na semana anterior seleciona
   8–10 finalistas, que apresentam os 5 min completos. **A arguição continua sendo de todos:** os
   não finalistas são arguidos em sessão agendada de 3 min por aluno, na semana de 30/11 a 04/12.
   Sem isso, a validação de autoria alcançaria só parte da turma e deixaria de ser instrumento.
2. **Duas rodadas.** Metade apresenta em uma data anterior. **Inviável em 2026.2**: 13/11 é
   meia-noite (Simulado 2) e 20/11 é feriado.
3. **Reduzir o pitch.** Voltar a 2 min (slot de 3min30) faz caber 17 alunos. É a saída que preserva
   “todo mundo apresenta no mesmo dia”, ao custo da profundidade do pitch.

> Em qualquer saída, o **minuto da arguição é intocável**. Ele é a única verificação presencial de
> autoria do semestre; cortá-lo para ganhar tempo esvazia a política de IA da disciplina.

**O aluno chega ao Demo Day com:**

- [ ] a aplicação **publicada** aberta numa aba (a demonstração é na versão no ar)
- [ ] o projeto **rodando localmente** e o **editor de código aberto** — a arguição acontece no código
- [ ] o repositório aberto no GitHub, na aba de commits
- [ ] plano B para queda de internet: um vídeo de 30 s da aplicação funcionando

> Chegar sem o código aberto não isenta da arguição: ela é remarcada como arguição individual
> (seção 6.1), o que só piora a situação do aluno.

Categorias:

- 🏆 Melhor projeto geral — **+5 pts**
- 🎨 Melhor UX / design — **+3 pts**
- 🧩 Código mais bem estruturado — **+3 pts**
- 💡 Mais criativo / inovador — **+3 pts**
- 🗳️ Escolha da turma (voto dos colegas) — **+3 pts**

> O bônus respeita o teto de 100 pontos na nota final. Confirmar o limite com a coordenação/NDE
> (Resolução 12 – CONSEPE 2023). Projeto zerado por qualquer item da seção 5 **não concorre** ao bônus.

---

## 11. Calendário do projeto — 2026.2

| Data | Marco |
|---|---|
| 07/08 | Aula 01 — regras do projeto, criação do repositório, primeiro commit, link na planilha |
| 14/08 | **Proposta de tema** commitada em `docs/proposta.md` (23h59) |
| 21/08 | Devolutiva e **aprovação do tema**; a partir daqui o tema está travado |
| 28/08 – 11/09 | Construção da Fase 1, um avanço commitado por aula prática |
| **18/09** | **Entrega da Fase 1** — 10 pts da AV1 |
| 25/09 | Devolutiva individual da Fase 1, após a prova AV1 — **arguição formativa** de 2 min (ensaio, não vale nota) |
| 09/10 – 23/10 | JavaScript aplicado ao site da Fase 1 |
| 30/10 – 13/11 | Migração para React, API, rotas, testes e deploy |
| 20/11 | Estudo Dirigido — finalização da Fase 2 e ensaio do pitch |
| **25/11** | **Entrega da Fase 2** — 20 pts da AV2 (23h59) |
| 27/11 | Prova AV + **Demo Day**: pitch de 5 min + **arguição técnica de 1 min** (valendo) |

