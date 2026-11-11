# API Biblioteca - Operacao Cracha

Projeto pratico da **Aula 08 - Autenticacao e Autorizacao com JWT** concluido.

## Endpoints e controle de acesso

| Metodo | Rota | Regra de acesso |
|---|---|---|
| GET | `/api/livros` | Publico |
| GET | `/api/livros/{id}` | Publico |
| POST | `/api/livros` | Exige token JWT (`[Authorize]`) |
| PUT | `/api/livros/{id}` | Exige token JWT (`[Authorize]`) |
| DELETE | `/api/livros/{id}` | Exige token JWT e role `Bibliotecario` (`[Authorize(Roles = "Bibliotecario")]`) |
| POST | `/api/auth/login` | Publico (emite o token JWT) |
| POST | `/api/auth/login-expirado` | Publico (emite token vencido para teste de `401`) |
| GET | `/api/quem-sou-eu` | Exige token JWT e lista as claims do usuario |

O detalhamento das etapas e dos testes encontra-se em `respostas.md`.
