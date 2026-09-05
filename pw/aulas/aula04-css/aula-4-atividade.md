# PERÍCIA DE ESTILO

*Como o navegador decide qual regra CSS ganha — e ele mostra isso de graça*

**Programação Web — Aula 4 de 20 · CSS: Seletores, Cascata e Box Model**

## 🎯 MISSÃO

O navegador não esconde nada: ele mostra quais regras aplicou, quais descartou e de onde veio cada valor final. Sua missão é aprender a ler esse relatório e, a partir dele, deduzir as regras do jogo.

- Escolha um site com visual elaborado (portal de notícias, loja, site institucional).
- Trabalhe na aba Elements do DevTools (F12): painéis Styles e Computed.
- Na Rodada 3, use o arquivo especificidade-quiz.html disponibilizado pelo professor.
- Notação de especificidade a usar: (id, classe, elemento). Ex.: #nav .item a = (1, 1, 1).

**⏱️ Tempo:** 40 minutos     **👥 Formato:** individual, conferindo cada rodada com o colega ao lado

> **Nome:** Guilherme Pereira Scatolino - 202251079375

## RODADA 01 — As regras que valem e as que morreram

> `Site real → botão direito num título → Inspecionar → painel Styles (lado direito)`

O painel Styles lista TODAS as regras que miram aquele elemento, da mais forte para a mais fraca. As que perderam aparecem riscadas. Catalogue o que você vê:

```text
elemento inspecionado: <h1>  class="content-head__title"

regras que VALEM (nao riscadas):
  1. seletor: font-size  propriedade: var(--g-fnt-size-110)
  2. seletor: letter-spacing  propriedade: var(--g-fnt-ls-340)

regras RISCADAS (perderam):
  1. seletor: font-size  propriedade: var(--g-fnt-size-80)
```

**Sua análise:**

1. Quantas regras diferentes tentavam estilizar esse único elemento?
> - Pelo menos 3: font-size; letter-spacing; line-height

1. Escolha uma regra riscada: por que você acha que ela perdeu?
> - font-size; perdeu por ser menos específica ou por ter sido sobrescrita por uma regra com maior especificidade

1. Existe alguma declaração com !important? Onde?
> - Nenhuma declaração !important

## RODADA 02 — De onde veio esse valor?

> `Mesmo elemento → painel Computed → clicar na setinha ao lado de uma propriedade`

O painel Computed mostra o valor FINAL de cada propriedade — inclusive de coisas que ninguém declarou. Investigue quatro delas:

```text
propriedade      valor final        veio de qual seletor?
--------------   ----------------   ----------------------
color            rgb(31, 33, 35)  --codex-clr-gray-170
font-size        40px               .glb-layout-ux2023
display          block              user-agent
margin-top       0px                reset CSS
```

**Sua análise:**

1. Alguma dessas propriedades tinha valor sem ninguém ter declarado nada? De onde ele veio?
>- Sim, algumas  aparecem sem ninguém declarar valor porque vêm do user-agent stylesheet

1. O valor de font-size aparece em px mesmo se o CSS usou outra unidade. Por que?
> - Porque o motor computa/resolve as metricas automaticamente

2. Qual propriedade dessa lista foi HERDADA do elemento pai?
> - Nesse caso, nenhuma

## RODADA 03 — Quem ganha — agora com a conta feita

> `Abrir especificidade-quiz.html e inspecionar o parágrafo de cada caixa`

Volte ao quiz do início da aula. Agora não é para adivinhar: conte os id, as classes e os elementos de cada seletor e escreva a soma antes de conferir no DevTools.

```text
cx  seletor vencedor            especificidade   cor final
--  --------------------------  --------------   ---------
 1  #alvo1                      (1 , 0 , 0)      verde
 2  #c2 p                       (1 , 0 , 1)      vermelho
 3  .empate                     (0 , 1 , 0)      verde
 4  style="..."                  (inline)        vermelho
 5  #alvo5                      (1 , 0 , 0)      verde
 6  .a6.b6                      (0 , 2 , 0)      verde
 7  #c5                         (1 , 0 , 0)      verde
 8  .card8 .destaque8 span      (0 , 2 , 1)      vermelho
```

**Sua análise:**

1. Na caixa 2, por que a regra com class perdeu para a regra com id + elemento?
> - 

2. Nas caixas 3, o que decidiu o resultado, se a especificidade era igual nas duas regras?
> -

3. Na caixa 7 nenhuma regra mirava o parágrafo. Então de onde veio a cor dele?
> -

## RODADA 04 — A caixa é maior do que você pediu

> `Site real → inspecionar um card ou botão → rolar o painel Styles até o fim → diagrama colorido do box model`

Todo elemento é uma caixa com quatro camadas. O diagrama do DevTools mostra as quatro. Anote as medidas e faça a conta à mão:

```text
                +---------------------------+
     margin     |  0 px                  |
                |  +---------------------+  |
     border     |  |  10 px            |  |
                |  |  +---------------+  |  |
     padding    |  |  |  20 px      |  |  |
                |  |  |  +---------+  |  |  |
     content    |  |  |  | 112 x 27 |  |  |  |
                |  |  |  +---------+  |  |  |

largura total ocupada = content + padding*2 + border*2 + margin*2
                      = 172  px
```

**Sua análise:**

1. Qual camada empurra os elementos vizinhos para longe, sem pintar nada?
> - Margin 

2. Qual camada aumenta a área clicável do elemento junto com o fundo?
> - Padding

3. A largura que aparece em width no CSS é a mesma que o elemento ocupa na tela?
> - Depende: content-box não; border-box sim

## RODADA 05 — O experimento do box-sizing

> `Ainda no elemento inspecionado → painel Styles → localizar (ou adicionar) box-sizing e alternar o valor`

Troque box-sizing entre content-box e border-box e observe o elemento na tela. Registre a diferença:

```text
width declarado no CSS: 823 px

box-sizing: content-box  ->  largura na tela: 898 px
box-sizing: border-box   ->  largura na tela: 823 px

diferenca entre as duas: 75 px
essa diferenca corresponde a que camadas? padding-left (20px) + padding-right (35px) + border-left (10px) + border-right (10px)
```

**Sua análise:**

1. Com qual dos dois valores a largura na tela é igual à largura que você declarou? 
> - Igual ao declarado com border-box.

2. Por que quase todo projeto começa o CSS com a regra * { box-sizing: border-box }? 
> - Porque border-box torna o dimensionamento previsível (width já inclui padding+border).

3. Se você somar padding a um elemento com border-box, o que muda de tamanho: a caixa ou o conteúdo dentro dela? 
> - Com border-box, ao somar padding o conteúdo encolhe; a caixa mantém a largura.

