# Proposta de Projeto - Programação Web

- **Aluno:** Guilherme Scatolino · **Curso/Turma:** Engenharia de Computação
- [**Repositório**](https://github.com/ScatolinoGui/Acervo-Historico-Celeste)

## 1. Tema e problema
O projeto será um acervo histórico dos ídolos do Cruzeiro Esporte Clube. O problema que ele resolve é a dificuldade de achar estatísticas e dados estruturados dos jogadores mais antigos do time em um só lugar, já que normalmente essas informações ficam espalhadas em sites diferentes e desorganizados.

## 2. Público-alvo
Torcedores do Cruzeiro, fãs de futebol em geral e pessoas que gostam de pesquisar sobre a história do esporte.

## 3. Coleção de itens
A coleção principal será de jogadores clássicos do time (mínimo de 8 registros). Cada jogador terá os seguintes atributos: Nome, Posição, Anos de Atuação, Gols Marcados e Títulos.
Exemplo de um item:
- Nome: Tostão
- Posição: Atacante
- Anos de Atuação: 1963-1972
- Gols Marcados: 242
- Títulos: Taça Brasil 1966 e 5 Campeonatos Mineiros

## 4. Telas previstas
1. Home (Listagem): A página inicial vai mostrar uma lista com os cards de todos os jogadores, exibindo foto, nome e posição.
2. Detalhe do Jogador: Uma tela específica para quando o usuário clicar no card de um jogador, mostrando a biografia e as estatísticas completas.
3. Indicação: Uma tela com um formulário para o usuário sugerir a adição de algum ídolo que esteja faltando no acervo.

## 5. Formulário
O formulário "Sugerir Ídolo" terá os seguintes campos:
1. Nome do jogador (input text, obrigatório)
2. Posição (select com opções, obrigatório)
3. Ano de Estreia (input number, com min e max)
4. Justificativa (textarea, com minlength obrigatório)
5. Nome de quem está indicando (input text, obrigatório)
6. Email (input email, obrigatório)

## 6. Filtro/busca
O usuário vai poder filtrar a lista de jogadores da página inicial pela posição em campo (Goleiro, Zagueiro, Meio-campo, Atacante).

## 7. Origem dos dados na Fase 2
Vou utilizar um mock local com json-server lendo um arquivo JSON para simular o banco de dados dos atletas.

## 8. Diferencial pretendido
Pretendo implementar um botão para alternar entre tema claro e escuro (Light/Dark mode) usando variáveis no CSS, e salvar a escolha do usuário no localStorage do navegador.