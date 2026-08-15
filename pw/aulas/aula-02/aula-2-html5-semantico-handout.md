# PERÍCIA DE ESTRUTURA

*O que o HTML diz sobre uma página quando ninguém está olhando a tela*

**Programação Web — Aula 2 de 20 · HTML5 Semântico**

## 🎯 MISSÃO

Duas páginas podem ser idênticas na tela e completamente diferentes por dentro. Sua missão é abrir sites reais e descobrir o que a marcação revela — ou esconde — sobre a estrutura do conteúdo.

- Escolha um site com bastante conteúdo (portal de notícias, blog, site institucional).
- Use a aba Elements do DevTools (F12) e, quando indicado, o painel Accessibility.
- Preencha à mão. Onde houver retângulo em branco, desenhe.
- Não existe gabarito: o que vale é a justificativa que você escreve.

**⏱️ Tempo:** 40 minutos     **👥 Formato:** individual, conferindo cada rodada com o colega ao lado

> **Nome:** Guilherme Pereira Scatolino   **Turma:** Programação Web   **Data:** 14/08/2026

## RODADA 01 — Div soup × semântico

> `Arquivos pagina-a.html e pagina-b.html (ambiente virtual da disciplina)`

Os dois trechos abaixo produzem exatamente a mesma tela. Um deles não diz nada sobre o que é cada parte. Para cada div numerada, escreva o elemento semântico que a substituiria.

```text
<!-- PAGINA A -->                     <!-- PAGINA B -->
<div class="topo">      (1) ______   <header>
  <div class="menu">    (2) ______     <nav>
<div class="miolo">     (3) ______   <main>
  <div class="post">    (4) ______     <article>
  <div class="lateral"> (5) ______     <aside>
<div class="rodape">    (6) ______   <footer>
```

**Sua análise:**

1. As duas páginas renderizam igual. O que exatamente a página B tem que a A não tem? <br>
A página B utiliza o HTML Semântico para estruturar a página no padrão que da sentido aos leitores/indexadores/IAs para facilitar o entendimento e organização do código da página.

2. Escolha UMA das div acima e explique como você decidiu qual elemento a substitui. <br>
Eu escolheria a div “miolo” e a substituiria por main. “miolo” é a parte central da página, onde fica o conteúdo principal. O elemento main foi criado exatamente para marcar essa região, indicando ao navegador, leitor de tela e mecanismos de busca. Isso deixa a estrutura mais clara e acessível, diferentemente de um simples div, que não comunica significado algum.

3. Sobrou algum caso em que o div é a escolha certa? Quando? <br>
Sim, o div ainda é a escolha certa quando a intenção é apenas agrupar conteúdo visual ou estrutural sem dar nenhum significado semântico específico.

## RODADA 02 — O mapa da página

> `Site real → DevTools → aba Elements → colapsar os nós e olhar só o primeiro nível dentro de <body>`

Desenhe no retângulo abaixo onde ficam as grandes regiões da página que você escolheu, escrevendo o nome do elemento que a marca (ou "div" se não houver elemento semântico):

```text
+--------------------------------------------------+
| header                                           |
|                                                  |
| nav - menu principal / áreas do portal           |
|                                                  |
| main - destaques, notícias, colunas e seções     |
|                                                  |
| aside - widgets, horóscopo, agenda, destaques    |
|                                                  |
| footer - links, redes, logo e informações        |
|                                                  |
+--------------------------------------------------+
  site investigado: globo.com.br
```

**Sua análise:**

1. Quantas regiões você conseguiu identificar sem abrir os nós filhos? <br>
header, nav, main, aside e footer

2. O site usa elementos semânticos ou div com class? Anote dois nomes de class que você viu. <br>
O site usa uma mistura dos dois. A estrutura principal é marcada com elementos semânticos, mas a maior parte do conteúdo é montada com divs e classes.
Encontrei header-section e area-destaque

3. Existe mais de um `<main>` na página? Deveria existir? <br>
Não vi mais de um <main> na estrutura principal da página. O ideal é ter apenas um <main>, porque ele representa o conteúdo principal da página e ajuda leitores de tela e mecanismos de busca a entenderem o foco da página.

## RODADA 03 — A hierarquia dos títulos

> `Ainda no mesmo site: no Console, digitar $$('h1,h2,h3,h4').map(h => h.tagName + ' ' + h.innerText.slice(0,40))`

Esse comando lista os títulos na ordem em que aparecem no código. Anote os primeiros e procure o problema:

```text
ordem   tag    texto do titulo
-----   ----   ------------------------------------
  1     h1     amanhã, 15/8
  2     h1     dom, 16/8
  3     h1     seg, 17/8
  4     h1     ter, 18/8
  5     h1     qua, 19/8
  ...
```

**Sua análise:**

1. Quantos h1 a página tem? Se tem mais de um, qual seria o problema disso? <br>
Possui 8 h1.
Isso é um problema porque o H1 deve representar o tema principal da página, e não repetir o mesmo nível para itens menores.

2. Algum nível foi pulado (um h2 seguido direto de um h4)? Anote onde. <br>
No conjunto apresentado, não aparece um caso óbvio de H2 direto para H4 em uma ordem clara.

3. Lendo só os títulos, você entende de que a página trata? Se não, o que está faltando? <br>
Lendo só os títulos, dá para perceber que a página é uma home de portal de notícias.

## RODADA 04 — O alt que ninguém lê (mas alguém ouve)

> `No Console: $$('img').slice(0,3).map(i => i.alt || '(SEM ALT)')`

Um leitor de tela lê o alt em voz alta no lugar da imagem. Anote os três primeiros e classifique cada um:

```text
img 1  alt = (SEM ALT)
       ( ) descritivo  ( ) inutil  (x) ausente  ( ) vazio proposital

img 2  alt = logo do usuário
       (x) descritivo  ( ) inutil  ( ) ausente  ( ) vazio proposital

img 3  alt = (SEM ALT)
       ( ) descritivo  ( ) inutil  (x) ausente  ( ) vazio proposital
```

**Sua análise:**

1. Algum alt era só o nome do arquivo ("banner-2024-final.jpg")? Por que isso é inútil? <br>
Não aparece isso. Isso seria inútil pois leitor de tela não consegue descrever o conteúdo delas.

2. Feche os olhos e imagine ouvir a página. O que você perderia com esses alt? <br>
Você perderia a informação de que há imagens importantes na página. Com alt ausente, a página fica incompleta para quem usa leitor de tela.

3. Reescreva o pior dos três de forma que descreva a imagem em menos de 12 palavras. <br>
Logo do portal Globo com destaque para notícias e entretenimento.

## RODADA 05 — O link fora de contexto

> `No Console: $$('a').slice(0,10).map(a => a.innerText.trim()).filter(t => t)`

Leitores de tela permitem navegar por uma lista só de links, sem o texto ao redor. Anote 3 textos de link e teste se sobrevivem sozinhos:

```text
link 1: "Ir para menu"  faz sentido sozinho? (x)sim ( )nao
link 2: "Ir para conteúdo principal"  faz sentido sozinho? (x)sim ( )nao
link 3: "Ir para rodapé"  faz sentido sozinho? (x)sim ( )nao
```

**Sua análise:**

1. Você encontrou algum "clique aqui", "saiba mais" ou "leia"? Para onde ele levava? <br>
Não. Os textos são claros e fazem sentido sozinhos.

2. Reescreva um desses textos para que ele diga o destino sem depender da frase ao redor. <br>
Ir para conteúdo principal -> Conteúdo principal da página

3. Algum link abria em nova aba? Como você descobriu isso olhando o código? <br>
Não, descobri lendo o próprio texto do link. Ele leva para locais dentro da mesma página.