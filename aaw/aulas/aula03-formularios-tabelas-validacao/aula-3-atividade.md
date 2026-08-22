# PERÍCIA EM FORMULÁRIOS

*Como a web coleta dados — e o que o navegador faz (e não faz) para protegê-los*

**Programação Web — Aula 3 de 20 · Formulários, Tabelas e Validação**

## 🎯 MISSÃO

Você vai testar formulários reais como um usuário desastrado e depois como alguém mal-intencionado. O objetivo é descobrir onde o HTML ajuda, onde ele atrapalha e até onde a proteção do navegador realmente vai.

- Use o arquivo formulario-hostil.html (ambiente virtual) e um formulário de cadastro real de um site à sua escolha.
- Trabalhe com o DevTools aberto (F12): abas Elements e Console.
- Nas rodadas 3 e 5, use o modo dispositivo (Ctrl+Shift+M) — ou o próprio celular.
- Preencha à mão. A Rodada 5 é a mais importante: não pule.

**⏱️ Tempo:** 40 minutos     **👥 Formato:** individual, conferindo cada rodada com o colega ao lado

> **Nome:** Guilherme P. Scatolino

## RODADA 01 — Anatomia de um campo

> `Formulário real → clicar com o botão direito num campo → Inspecionar`

Cada campo de formulário é uma tag input com atributos que decidem tudo. Catalogue três campos do formulário que você escolheu:

```text
campo   type=text  name=nome
  1     id=c1  required? ( )sim (x)nao

campo   type=text  name=email
  2     id=c2  required? ( )sim (x)nao

campo   type=text  name=telefone
  3     id=c3  required? ( )sim (x)nao
```

**Sua análise:**

1. Os atributos name e id têm o mesmo valor nos campos que você viu? Eles servem para a mesma coisa?
Resposta: Não. O id identifica o elemento no HTML e serve para o label e para JavaScript; o name identifica o campo quando o formulário é enviado.

2. Algum campo usa placeholder em vez de um rótulo visível? Qual o problema disso?
Resposta: Não há rótulo visível em alguns campos; placeholder não substitui o texto do label, porque ele desaparece ao digitar e não é tão claro para acessibilidade.

3. Que tipo de dado cada campo espera receber, só pelo type?
Resposta: Os campos são texto, pois todos usam type="text"; o navegador não diferencia semântica de e-mail, telefone ou idade sem o tipo correto.

## RODADA 02 — O teste do label

> `Clicar no TEXTO do rótulo (não no campo) em formulario-hostil.html e depois no formulário real`

Um label corretamente associado faz o clique no texto focar o campo. É um teste de 1 segundo que revela se a marcação está certa:

```text
formulario-hostil.html   clicar no rotulo focou o campo? ( )sim (x)nao
formulario real          clicar no rotulo focou o campo? (x)sim ( )nao

codigo do que FUNCIONA:
  <label for="nome">Nome</label>
  <input id="nome">
```

**Sua análise:**

1. Qual atributo do label precisa bater com qual atributo do input?
Resposta: O atributo for do label precisa bater com o id do input.

2. Além do clique, quem mais depende dessa associação para saber o nome do campo?
Resposta: Leitores de tela e tecnologias assistivas dependem dessa associação para anunciar corretamente o nome do campo.

3. No formulário hostil, o que exatamente estava faltando?
Resposta: Estava faltando o label associado ao input; o texto estava em span/div e não havia for/id conectando os elementos.

## RODADA 03 — O teclado que o celular abre

> `DevTools → Ctrl+Shift+M (modo dispositivo) ou abra a página no seu celular`

O atributo type muda o teclado que aparece no celular. Teste cada um e descreva o que apareceu:

```text
type="text"      teclado: teclado alfanumérico
type="email"     teclado: teclado com @ e .com
type="tel"       teclado: teclado numérico/telefone
type="number"    teclado: teclado numérico
type="date"      controle: calendário/seleção de data
```

**Sua análise:**

1. Qual type você usaria para CEP? E para um valor em reais? Justifique.
Resposta: Para CEP, eu usaria type="text" ou inputmode="numeric" com máscara, porque CEP pode ter zeros à esquerda; para valor em reais, type="number" com step="0.01".

2. Um campo de telefone com type="text" funciona. Então por que usar type="tel"?
Resposta: Porque type="tel" abre o teclado numérico adequado e comunica melhor a intenção do campo para o celular e para o navegador.

3. O type="date" mostrou um calendário? Quem desenhou esse calendário: você ou o navegador?
Resposta: O navegador desenhou o calendário/controle nativo do sistema, não o desenvolvedor.

## RODADA 04 — A validação que vem de graça

> `Formulário real → deixar tudo em branco → clicar em enviar`

Antes de qualquer JavaScript, o navegador já barra o envio. Anote a mensagem EXATA que apareceu e descubra quem a causou:

```text
mensagem exibida pelo navegador:
  "Preencha este campo."

campo que ele apontou primeiro: primeiro campo obrigatório
atributo no HTML que causou isso: required

agora digite "batata" num campo type="email" e envie:
  o que aconteceu? O navegador bloqueia o envio e informa que o e-mail está inválido.
```

**Sua análise:**

1. Você escreveu alguma linha de JavaScript para isso funcionar?
Resposta: Não. A validação foi feita pelo próprio navegador, sem JavaScript.

2. A mensagem apareceu em português. Quem escolheu esse idioma?
Resposta: O navegador e o sistema operacional definiram o idioma da mensagem.

3. Qual atributo você usaria para exigir no mínimo 3 caracteres num campo de nome?
Resposta: Eu usaria minlength="3".

## RODADA 05 — Burlando a validação (a rodada que importa)

> `DevTools → Elements → achar um input com required → duplo clique no atributo → apagar → Enter → submeter vazio`

Você acabou de remover a proteção do formulário sem instalar nada, em 3 segundos, só com o navegador. Registre o resultado:

```text
antes de apagar o required, o envio vazio era: (x)bloqueado ( )permitido
depois de apagar o required, o envio vazio foi: ( )bloqueado (x)permitido

tempo que você levou para fazer isso: 3 segundos
ferramenta extra que voce precisou instalar: nenhuma
```

**Sua análise:**

1. Se qualquer pessoa faz isso em 3 segundos, a validação do HTML serve para proteger o SISTEMA ou para ajudar o USUÁRIO?
Resposta: Ela serve para ajudar o usuário, não para proteger o sistema.

2. Onde, então, a validação precisa acontecer de novo obrigatoriamente?
Resposta: A validação precisa acontecer no backend, no servidor.

3. Escreva em uma frase o que você diria a um colega que afirma "meu formulário está seguro, tem required em tudo".
Resposta: Requerido no HTML ajuda a UX, mas segurança real exige validação no servidor e tratamento de dados no backend.

## RODADA 06 — A tabela é de dados ou de layout?

> `Procurar uma tabela real (extrato bancário, tabela de preços, classificação de campeonato) → Inspecionar`

Tabela serve para dados tabulares, com cabeçalhos que dizem o que cada coluna significa. Verifique se a que você achou está marcada corretamente:

```text
usa <caption> (titulo da tabela)?     (x)sim ( )nao
usa <thead> e <tbody>?                (x)sim ( )nao
os cabecalhos sao <th> ou <td>?       (x)th  ( )td
os <th> tem scope="col" ou "row"?     (x)sim ( )nao

site investigado: https://www.bcb.gov.br/
```

**Sua análise:**

1. Se os cabeçalhos são td, como um leitor de tela sabe que "R$ 250,00" pertence à coluna Valor?
Resposta: Ele não sabe; sem th e sem associação correta, a leitura da tabela fica confusa e menos acessível.

2. Para que serve o atributo scope?
Resposta: O scope indica se o cabeçalho pertence a uma coluna ou a uma linha, ajudando o leitor de tela a identificar a relação entre células e títulos.

3. Você encontrou alguma tabela usada para posicionar elementos na tela em vez de mostrar dados? Por que isso é um problema?
Resposta: Sim, quando a tabela é usada para layout, a semântica fica errada e a acessibilidade fica prejudicada; a estrutura deixa de refletir dados e passa a confundir leitores de tela.