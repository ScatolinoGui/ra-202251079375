# Resolucao da atividade pratica - Aula 06 (Comunicacao entre servicos e filas)

## Parte 1: Handout - Sincrono ou assincrono?

### Cenario 01 - PagFacil (aprovar ou negar agora)
1. **Estilo recomendado**: `[x] Sincrono (request/response)`
2. **Fluxo**: `[Cliente] -> (POST /pagar) -> [Pagamentos] -> (GET /contas/saldo) -> [Contas]`
3. **Justificativa**:
   - **Decisao bloqueante de negocio**: a venda depende imediatamente do retorno de saldo/limite para aprovar ou recusar na tela de checkout.
   - **Baixa latencia previsivel**: o servico de Contas responde em ~80 ms, viabilizando chamada direta sem degradar a experiencia do comprador.
4. **Principal risco**: acoplamento temporal (se `Contas` ficar indisponivel ou lento, o checkout de `Pagamentos` falha em cascata, exigindo *timeout* curto e *circuit breaker*).

### Cenario 02 - CadastraJa (o e-mail de boas-vindas)
1. **Estilo recomendado**: `[x] Assincrono (fila/evento)`
2. **Fluxo**: `[Cliente] -> [UsuariosApi] -> (Publica UsuarioCriado) -> [Fila] -> [Worker Notificacao] -> [Provedor SMTP]`
3. **Justificativa**:
   - **Tolerancia a atraso (consistencia eventual)**: o usuario precisa entrar no aplicativo na hora, enquanto o e-mail pode chegar segundos ou minutos depois.
   - **Isolamento de falhas e lentidao externa**: o provedor demora ate 8s e falha em 2% das chamadas; a fila permite *retry* transparente em segundo plano sem travar o cadastro.
4. **Principal risco**: perda de mensagens se a fila for apenas em memoria ou entrega duplicada caso o consumidor nao seja idempotente.

### Cenario 03 - MegaMarket (baixa de estoque nos picos)
1. **Estilo recomendado**: `[x] Assincrono (fila/evento)`
2. **Fluxo**: `[Checkout] -> (Publica VendaConfirmada) -> [Broker/Fila Duravel] -> [EstoqueWorker]`
3. **Justificativa**:
   - **Amortecimento de picos (*load leveling*)**: quando o trafego sobe 10x, a fila absorve a rajada e o servico de Estoque consome na sua capacidade maxima sem cair.
   - **Durabilidade**: um broker persistente (RabbitMQ/Kafka) garante que nenhuma baixa seja perdida mesmo sob alta carga.
4. **Principal risco**: janela de *overselling* (vender item cujo estoque acabou alguns segundos antes de a fila drenar), mitigada com reserva previa ou compensacao (*Saga*).

### Cenario 04 - AppBanco (uma tela, cinco servicos)
1. **Estilo recomendado**: `[x] API Gateway / BFF (Backend for Frontend)`
2. **Fluxo**: `[App Mobile] -> (1 chamada GET /home) -> [BFF Mobile] -> (chamadas internas paralelas) -> [Saldo, Cartao, Investimentos, Emprestimos, Cashback]`
3. **Justificativa**:
   - **Reducao de *round-trips* na rede movel**: o celular faz uma unica requisicao ao BFF, que agrega os 5 servicos em rede interna de baixa latencia.
   - **Contrato sob medida por canal**: o BFF Mobile entrega apenas o payload enxuto do celular, enquanto um BFF Web futuro pode agregar mais dados sem alterar os microsservicos internos.
4. **Principal risco**: o BFF virar ponto unico de falha (*Single Point of Failure*) ou acumular regras de negocio de dominio que deveriam estar nos servicos.

### Desafio: Entrega "pelo menos uma vez" (*At-least-once*) e idempotencia
- No **Cenario 03 (MegaMarket)**, se a mesma mensagem de baixa chegar duas vezes, o estoque seria decrementado em dobro.
- **Protecao no consumidor (Consumidor Idempotente)**: cada mensagem carrega um `EventoId` (ou `PedidoId`) unico. Antes de baixar o estoque, o consumidor verifica em uma tabela de eventos processados (na mesma transacao do banco) se aquele `EventoId` ja foi aplicado; se ja existir, descarta a duplicata e confirma o `ACK`.

---

## Parte 2: Reflexao da pratica `LojaDev` (Fase 5)

1. **Por que o pagamento e sincrono nos dois fluxos?** Porque a aprovacao do pagamento define se o pedido pode ou nao ser aceito; nao faz sentido publicar evento de pedido aprovado nem enviar e-mail de confirmacao se o cartao for recusado (`valor > 10000`).
2. **O que acontece se a fila perder um evento (`Channel<T>` em memoria)?** Como `Channel<T>` vive na RAM do processo `LojaApi`, se o processo reiniciar antes de o `NotificacaoBackground` consumir os itens pendentes, esses eventos sao perdidos e os clientes ficam sem e-mail.
3. **Em producao, o que substituiria o `Channel<T>`?** Um message broker externo com persistencia em disco e confirmacao explicita (`ACK`/`NACK`), como **RabbitMQ**, **Apache Kafka**, **Azure Service Bus** ou **Amazon SQS**.
4. **Se a `NotificacaoApi` falhar, como o consumidor da fila deveria reagir?** Aplicar retentativas com atraso progressivo (*retry with exponential backoff*) para falhas transientes e, apos atingir o limite de tentativas, mover a mensagem para uma **Dead-Letter Queue (DLQ)** para inspecao e reprocessamento sem travar a fila principal.
5. **Comparacao dos tempos de resposta medidos**:

| Fluxo | Tempo de resposta | Notificacao chega? |
|-------|-------------------|--------------------|
| Sincrono (`POST /api/pedidos/sincrono`) | ~4.000 ms (1s pagamento + 3s notificacao) | Imediata na mesma requisicao (mas pode falhar em 20% dos casos ou lancar erro se `NotificacaoApi` cair) |
| Assincrono (`POST /api/pedidos/assincrono`) | ~1.000 ms (apenas o pagamento; retorna `202 Accepted`) | Processada em background ~3s depois pelo `NotificacaoBackground` |
