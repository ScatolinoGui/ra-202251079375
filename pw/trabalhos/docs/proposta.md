# Proposta de Projeto — Programação Web 2026.2

- **Aluno:** Guilherme Scatolino · **Curso/Turma:** Engenharia de Computação
- **Repositório:** https://github.com/ScatolinoGui/Acervo-Historico-Celeste

## 1. Tema e problema
O projeto é um acervo histórico dos ídolos do Cruzeiro Esporte Clube. Ele resolve a dificuldade de encontrar estatísticas consolidadas e dados estruturados dos jogadores mais antigos, centralizando essas informações que normalmente ficam espalhadas na internet.

## 2. Público-alvo
Torcedores do Cruzeiro, fãs de futebol em geral e pessoas que gostam de pesquisar sobre a história do esporte.

## 3. Coleção de itens
A coleção principal será de jogadores clássicos do time (mínimo de 8 registros). Cada item terá os seguintes atributos: Foto, Nome, Posição, Anos de Atuação, Gols Marcados e Títulos.

**Exemplo preenchido:**
- Nome: Tostão
- Posição: Atacante
- Anos de Atuação: 1963-1972
- Gols Marcados: 242
- Títulos: Taça Brasil 1966 e 5 Campeonatos Mineiros

## 4. Telas previstas
1. **Home/Landing Page:** Apresentação do projeto.
2. **Galeria:** Lista com os cards de todos os jogadores (foto, nome e posição).
3. **Detalhes:** Tela específica do jogador clicado, mostrando estatísticas completas.
4. **Sugerir Ídolo:** Formulário para indicar jogadores faltantes no acervo.

## 5. Formulário
O formulário de "Sugerir Ídolo" terá os seguintes campos e validações nativas:
1. Nome do jogador: `type="text"`, `required`
2. Posição: `<select>`, `required`
3. Ano de Estreia: `type="number"`, `min="1921"`, `max="2026"`, `required`
4. Justificativa: `<textarea>`, `minlength="20"`, `required`
5. Nome de quem indica: `type="text"`, `required`
6. Email: `type="email"`, `pattern` para validar formato, `required`

## 6. Filtro/busca
O usuário poderá buscar os jogadores na Galeria digitando o nome do ídolo, além de poder filtrar a lista pela posição em campo (Ex: Goleiro, Zagueiro, Meio-campo, Atacante).

## 7. Origem dos dados na Fase 2
Mock local utilizando `json-server` consumindo um arquivo JSON (`db.json`).

## 8. Diferencial pretendido
Pretendo implementar um botão para alternar entre tema claro e escuro (Light/Dark mode) usando variáveis no CSS e salvando a escolha no `localStorage`, além de desenvolver todo o design system (Grid/Flexbox) na mão, sem frameworks CSS.