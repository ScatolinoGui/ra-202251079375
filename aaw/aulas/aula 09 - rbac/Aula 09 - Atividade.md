# HANDOUT — AULA 09

## Projete a Matriz de Acesso

*SisAcad — sistema acadêmico fictício — Arquitetura de Aplicações Web*

## 🎯 MISSÃO

Vocês são o time de segurança contratado para definir QUEM pode fazer O QUÊ no SisAcad, o novo sistema acadêmico da faculdade. Os papéis já existem: Aluno, Professor, Coordenador e Secretaria.

- Preencham a matriz abaixo com ✓ (pode) ou ✗ (não pode) para cada papel × ação
- Justifiquem por escrito os 3 casos que geraram mais discussão na dupla
- Respondam à pergunta-armadilha da Parte 03 — ela esconde o limite do modelo

## PARTE 01 — A matriz papel × permissão

Marquem ✓ (pode) ou ✗ (não pode) em cada célula:

| **Ação**                  | **Aluno** | **Professor** | **Coordenador** | **Secretaria** |
| --------------------------------- | --------------- | ------------------- | --------------------- | -------------------- |
| 1. Ver as próprias notas         | ✓              | ✗                  | ✗                    | ✗                   |
| 2. Ver notas de toda a turma      | ✗              | ✓                  | ✓                    | ✗                   |
| 3. Lançar nota                   | ✗              | ✓                  | ✗                    | ✗                   |
| 4. Alterar nota já lançada      | ✗              | ✗                  | ✓                    | ✗                   |
| 5. Matricular aluno em disciplina | ✗              | ✗                  | ✓                    | ✓                   |
| 6. Trancar matrícula             | ✗              | ✗                  | ✓                    | ✓                   |
| 7. Criar nova disciplina          | ✗              | ✗                  | ✓                    | ✗                   |
| 8. Ver dados pessoais de aluno    | ✗              | ✗                  | ✓                    | ✓                   |
| 9. Emitir histórico escolar      | ✗              | ✗                  | ✓                    | ✓                   |
| 10. Aprovar colação de grau     | ✗              | ✗                  | ✓                    | ✗                   |

*Dica: pensem em consequências — quem responde perante o MEC se uma nota for alterada indevidamente? E perante a LGPD se dados pessoais vazarem?*

## PARTE 02 — Os casos polêmicos

Quais 3 células geraram mais discussão na dupla? Anotem a célula (ação + papel) e a justificativa da decisão final:

1. Caso 1: Profesor deve alterar a nota ja lançada? A alteração de uma nota já registrada deve ser controlada com muita atenção, porque afeta a integridade acadêmica e pode gerar fraude ou erro.
2. Caso 2: Coordenador pode ver os dados pessoais do aluno? No geral sim, mas deve ser somente restrito a situações academicas, para mitigar o vazamento de dados sensíveis.
3. Caso 3: Lançar nota - O professor pode lançar notas da turma que ele leciona, mas não de qualquer disciplina. Se o modelo permitir “Professor = pode lançar nota em qualquer turma”, abre brecha para abuso de acesso e erros graves. Por isso, a permissão precisa depender do contexto da disciplina/turma, não apenas do papel.

## PARTE 03 — A pergunta-armadilha

Vocês marcaram que Professor pode “lançar nota”. Mas… de QUALQUER turma? O professor de Cálculo pode lançar nota na turma de Arquitetura de Aplicações Web?

1. O papel “Professor” sozinho é suficiente para decidir esse acesso? Por quê?
   Não. O papel sozinho não basta, porque “ser professor” não significa “poder mexer em qualquer turma”. O sistema precisa saber se aquele professor é responsável pela disciplina/turma em questão.
2. Que informação ALÉM do papel o sistema precisa conhecer para decidir corretamente?
   O sistema precisa conhecer a relação entre professor e turma/disciplina. Sem essa informação, o acesso fica inseguro.
3. Onde vocês já viram esse mesmo problema nesta disciplina? (dica: Aula 07…)
   Esse é o mesmo problema de autorização baseada apenas em papel sem considerar contexto/escopo de acesso.

## DESAFIO

1. Chegou um pedido da coordenação: criar o papel “Monitor” — um aluno que ajuda o professor em uma disciplina. Que permissões vocês dariam ao Monitor? Que risco novo esse papel cria?

	Permissões sugeridas:
		Acessar dados acadêmicos do grupo específico da disciplina;

	Risco novo:
		Expansão indevida de privilégios;
		Vazamento de dados sensíveis de alunos;
		Necessidade de controle por disciplina/turma, para que o monitor só veja o contexto de sua atividade.
