# Resolucao da atividade pratica - Aula 07 (Seguranca Web e OWASP Top 10)

## Vulnerabilidade 01 - Busca de clientes / produtos (`GET /api/produtos/buscar?nome=...`)
1. **Qual e a falha?** Injecao de SQL (*SQL Injection* - OWASP Injection). O parametro `nome` e concatenado diretamente na string SQL executada via `FromSqlRaw`, fazendo o banco interpretar texto do usuario como comando.
2. **Dano possivel em producao**: bypass de filtros (`%' OR '1'='1`), leitura completa de tabelas confidenciais ou adulteracao/exclusao de registros caso o comando permita multiplas instrucoes.
3. **Como corrigir?** Separar comando de dado usando consultas parametrizadas (`.FromSqlRaw("SELECT * FROM Produtos WHERE Nome LIKE {0}", $"%{nome}%")`, `FromSqlInterpolated` ou LINQ `.Where(p => p.Nome.Contains(nome))`).

## Vulnerabilidade 02 - Consulta de faturas / pedidos (`GET /api/pedidos/{id}`)
1. **Qual e a falha?** IDOR (*Insecure Direct Object Reference* / *Broken Access Control* - OWASP #1). O atributo `[Authorize]` verifica apenas se o usuario esta autenticado, mas o endpoint nao confere se o recurso solicitado pertence ao usuario logado.
2. **Dano possivel em producao**: qualquer cliente autenticado consegue enumerar IDs sequenciais na URL (`101`, `102`, `103`) e acessar faturas, dados pessoais e pedidos de terceiros.
3. **Como corrigir?** Extrair o identificador do usuario a partir das claims do token JWT (`User.FindFirstValue(ClaimTypes.NameIdentifier)`) e comparar `pedido.UsuarioId != UsuarioLogadoId`, retornando `403 Forbidden` (`Forbid()`).

## Vulnerabilidade 03 - Configuracao do servidor (`Program.cs`)
1. **Qual e a falha?** *Security Misconfiguration* e *Mishandling of Exceptional Conditions*. Credenciais de banco e chave secreta JWT estao fixas em constantes no codigo versionado no Git, e `app.UseDeveloperExceptionPage()` esta habilitado incondicionalmente em producao.
2. **Dano possivel em producao**: quem tiver acesso ao repositorio consegue acesso administrativo ao banco e pode forjar tokens JWT para qualquer perfil; além disso, erros em producao vazam *stack trace*, caminhos internos e versoes de bibliotecas.
3. **Como corrigir?** Ler segredos via `IConfiguration` (variaveis de ambiente, `dotnet user-secrets` ou cofre de chaves, rotacionando chaves ja expostas) e restringir `UseDeveloperExceptionPage()` ao bloco `if (app.Environment.IsDevelopment())`, usando `app.UseExceptionHandler("/erro")` com `Results.Problem("Erro interno.")` em producao.

## Vulnerabilidade 04 - Atualizacao de perfil (`PUT /api/usuarios/{id}`)
1. **Qual e a falha?** *Mass Assignment* / *Over-posting* (escalacao vertical de privilegio dentro de *Broken Access Control*). O endpoint faz *model binding* direto da entidade `Usuario` (ou de um DTO que inclui `Role`), permitindo ao proprio cliente enviar `"role": "Admin"` no JSON.
2. **Dano possivel em producao**: um usuario comum promove a propria conta para administrador e ganha acesso total a operacoes restritas.
3. **Como corrigir?** Receber um DTO de entrada restrito apenas aos campos editaveis pelo usuario (`record AtualizarUsuarioRequest(string Nome, string Email)`), validar a titularidade (`usuario.Id != UsuarioLogadoId -> Forbid()`) e nunca atribuir `Role` nessa rota.

## Desafio
- **Qual das 4 falhas um scanner automatico (SAST) teria mais dificuldade de encontrar?**
  - A **Vulnerabilidade 02 (IDOR / Broken Access Control)** (seguida da 04). Scanners estaticos detectam facilmente padroes sintaticos como concatenacao de strings em `FromSqlRaw`, constantes com nomes de senha ou `UseDeveloperExceptionPage()`, mas nao compreendem a regra de negocio de quem pode ver qual registro, pois o codigo tem `[Authorize]` e parece sintaticamente normal.
