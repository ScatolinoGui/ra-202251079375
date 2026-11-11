# Resolucao da atividade pratica - Aula 09 (Controle de acesso RBAC, ABAC e PBAC)

## Parte 1: Handout SisAcad - Matriz de acesso

| Acao | Aluno | Professor | Coordenador | Secretaria |
| --- | :---: | :---: | :---: | :---: |
| 1. Ver as proprias notas | Sim | Nao | Nao | Nao |
| 2. Ver notas de toda a turma | Nao | Sim (sua turma) | Sim (seu curso) | Sim |
| 3. Lancar nota | Nao | Sim (sua turma) | Nao | Nao |
| 4. Alterar nota ja lancada | Nao | Nao | Sim (com trilha) | Nao |
| 5. Matricular aluno em disciplina | Sim (periodo aberto) | Nao | Sim (excecao) | Sim |
| 6. Trancar matricula | Sim (dentro do prazo) | Nao | Sim | Sim |
| 7. Criar nova disciplina | Nao | Nao | Sim | Sim |
| 8. Ver dados pessoais de aluno | Sim (proprios) | Nao | Nao | Sim |
| 9. Emitir historico escolar | Sim (proprio) | Nao | Nao | Sim |
| 10. Aprovar colacao de grau | Nao | Nao | Sim | Sim |

### Parte 2 - Tres casos discutidos
1. **Alterar nota ja lancada (Professor vs Coordenador)**: apos o fechamento da pauta, o professor nao deve alterar nota diretamente sem processo formal; restringir ao Coordenador com registro de auditoria evita fraudes perante o MEC.
2. **Ver dados pessoais de aluno (Professor e Coordenador)**: pela LGPD (principio da necessidade / privilegio minimo), CPF, endereco e telefone pertencem ao cadastro administrativo da Secretaria e ao proprio aluno, nao sendo necessarios para lecionar.
3. **Matricular aluno em disciplina (Aluno vs Secretaria)**: o aluno pode solicitar matricula nos proprios horarios durante a janela oficial, enquanto a Secretaria executa matriculas administrativas e ajustes.

### Parte 3 - A pergunta-armadilha (RBAC vs ABAC)
1. **O papel "Professor" sozinho e suficiente?** Nao. O RBAC puro enxerga apenas que o sujeito tem a role `Professor`, o que permitiria ao professor de Calculo lancar notas na turma de Arquitetura de Aplicacoes Web.
2. **Que informacao adicional o sistema precisa?** Precisa combinar atributos do sujeito (`professor.Id` no token) com atributos do recurso (`turma.ProfessorResponsavelId` no banco) e do contexto (se o semestre/diario esta aberto), caracterizando **ABAC**.
3. **Conexao com aulas anteriores**: e exatamente o problema de **IDOR / Broken Access Control** visto na Aula 07 (`PedidosController`).

### Desafio (Papel "Monitor")
- **Permissoes do Monitor**: visualizar lista de presença e materiais da turma específica onde é monitor (somente leitura, sem poder lançar ou alterar notas e sem ver dados pessoais sensíveis).
- **Novo risco**: conflito de interesse (o monitor também é `Aluno` em outras disciplinas) e vazamento de notas de colegas caso o controle não seja filtrado por vínculo ABAC (`monitor.TurmaAlocadaId == turma.Id`).

---

## Parte 2: As 4 falhas corrigidas na `BibliotecaApi`

1. **Falha 1 (RBAC desalinhado com a matriz)**: criadas as policies `"GerenciarAcervo"` (`Bibliotecario` e `Admin` para `POST`/`PUT`) e `"RemoverLivro"` (somente `Admin` para `DELETE`) em `Program.cs` e aplicadas em `LivrosController.cs`.
2. **Falha 2 (Ausencia de negar por padrao)**: configurada `FallbackPolicy` exigindo usuario autenticado por padrao em `Program.cs` e aplicada `[Authorize(Policy = "VerRelatorios")]` (restrita a `Admin`) em `RelatoriosController.cs`.
3. **Falha 3 (ABAC em `DonoDoEmprestimoHandler`)**: o handler passou a aprovar se `context.User.IsInRole("Bibliotecario")` ou se a claim `ClaimTypes.NameIdentifier` do sujeito for igual a `emprestimo.UsuarioId.ToString()` do recurso.
4. **Falha 4 (PBAC em `politicas.json`)**: a regra especifica de `Categoria: "ObraRara"` foi posicionada antes da regra curinga `Categoria: "*"`, evitando o sombreamento (*rule shadowing*) na avaliacao *first-applicable*.
