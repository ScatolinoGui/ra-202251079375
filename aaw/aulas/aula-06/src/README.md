# Aula 06 - Comunicacao entre Servicos - Pratica

Comparacao pratica entre comunicacao sincrona e assincrona com fila em memoria (`Channel<T>`) e `BackgroundService`.

## Cenario: LojaDev

| Servico | Porta | O que faz |
|---------|-------|-----------|
| **LojaApi** | 5090 | Recebe pedidos do cliente e orquestra pagamento e notificacao |
| **PagamentoApi** | 5091 | Aprova ou rejeita pagamentos (~1s de latencia) |
| **NotificacaoApi** | 5092 | Envia e-mail (~3s de latencia, ~20% de falha simulada) |

## Conteudo desta pasta

| Item | O que e |
|------|---------|
| `LojaDev.slnx` | Solution com os projetos da pratica |
| `LojaDev.Compartilhado/` | Modelos compartilhados e fila em memoria (`Channel<T>`) |
| `LojaDev.PagamentoApi/` | Servico de pagamento |
| `LojaDev.NotificacaoApi/` | Servico de notificacao |
| `LojaDev.LojaApi/` | Servico principal com os fluxos `/api/pedidos/sincrono` e `/api/pedidos/assincrono` |
| `respostas.md` | Resolucao dos 4 cenarios arquiteturais e das questoes de reflexao da Fase 5 |
