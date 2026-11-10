using System.Text;
using System.Text.Json;
using LojaDev.Compartilhado.Fila;
using LojaDev.Compartilhado.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace LojaDev.LojaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;
    private readonly FilaDePedidos _fila;

    public PedidosController(
        IHttpClientFactory httpFactory,
        IConfiguration config,
        FilaDePedidos fila)
    {
        _httpFactory = httpFactory;
        _config = config;
        _fila = fila;
    }

    [HttpPost("sincrono")]
    public async Task<IActionResult> CriarPedidoSincrono([FromBody] Pedido pedido)
    {
        var inicioTotal = DateTime.Now;
        Console.WriteLine($"\nFLUXO SINCRONO - Pedido {pedido.Id}");

        var urlPagamento = _config["ServicoPagamento"];
        var client = _httpFactory.CreateClient();
        var json = JsonSerializer.Serialize(pedido);
        var conteudo = new StringContent(json, Encoding.UTF8, "application/json");
        var resposta = await client.PostAsync($"{urlPagamento}/api/pagamentos", conteudo);
        var corpo = await resposta.Content.ReadAsStringAsync();
        var resultado = JsonSerializer.Deserialize<JsonElement>(corpo);
        bool aprovado = resultado.GetProperty("aprovado").GetBoolean();

        if (!aprovado)
        {
            var tempoRejeicao = (DateTime.Now - inicioTotal).TotalMilliseconds;
            Console.WriteLine($"Pedido REJEITADO em {tempoRejeicao:F0}ms");
            return BadRequest(new
            {
                pedidoId = pedido.Id,
                status = "rejeitado",
                mensagem = "Pagamento não aprovado",
                tempoTotalMs = tempoRejeicao
            });
        }

        var urlNotificacao = _config["ServicoNotificacao"];
        var bodyNotificacao = new
        {
            Destinatario = pedido.Cliente,
            Assunto = $"Pedido {pedido.Id} confirmado!",
            Corpo = $"Seu pedido de {pedido.Produto} (R$ {pedido.Valor}) foi aprovado."
        };
        var jsonNotif = JsonSerializer.Serialize(bodyNotificacao);
        var conteudoNotif = new StringContent(jsonNotif, Encoding.UTF8, "application/json");
        var respostaNotif = await client.PostAsync($"{urlNotificacao}/api/notificacoes", conteudoNotif);
        string statusNotificacao = respostaNotif.IsSuccessStatusCode ? "enviada" : "falhou";

        var tempoTotal = (DateTime.Now - inicioTotal).TotalMilliseconds;
        Console.WriteLine($"SINCRONO concluido em {tempoTotal:F0}ms\n");

        return Ok(new
        {
            pedidoId = pedido.Id,
            status = "aprovado",
            notificacao = statusNotificacao,
            fluxo = "sincrono",
            tempoTotalMs = tempoTotal,
            observacao = "Tempo total inclui a espera sincrona da notificacao."
        });
    }

    [HttpPost("assincrono")]
    public async Task<IActionResult> CriarPedidoAssincrono([FromBody] Pedido pedido)
    {
        var inicioTotal = DateTime.Now;
        Console.WriteLine($"\nFLUXO ASSINCRONO - Pedido {pedido.Id}");

        var urlPagamento = _config["ServicoPagamento"];
        var client = _httpFactory.CreateClient();
        var json = JsonSerializer.Serialize(pedido);
        var conteudo = new StringContent(json, Encoding.UTF8, "application/json");
        var resposta = await client.PostAsync($"{urlPagamento}/api/pagamentos", conteudo);
        var corpo = await resposta.Content.ReadAsStringAsync();
        var resultado = JsonSerializer.Deserialize<JsonElement>(corpo);
        bool aprovado = resultado.GetProperty("aprovado").GetBoolean();

        if (!aprovado)
        {
            var tempoRejeicao = (DateTime.Now - inicioTotal).TotalMilliseconds;
            Console.WriteLine($"Pedido REJEITADO em {tempoRejeicao:F0}ms");
            return BadRequest(new
            {
                pedidoId = pedido.Id,
                status = "rejeitado",
                mensagem = "Pagamento não aprovado",
                tempoTotalMs = tempoRejeicao
            });
        }

        var evento = new EventoPedidoAprovado
        {
            PedidoId = pedido.Id,
            Cliente = pedido.Cliente,
            Produto = pedido.Produto,
            Valor = pedido.Valor
        };
        await _fila.PublicarAsync(evento);

        var tempoTotal = (DateTime.Now - inicioTotal).TotalMilliseconds;
        Console.WriteLine($"ASSINCRONO concluido em {tempoTotal:F0}ms (notificacao em background)\n");

        return Accepted(new
        {
            pedidoId = pedido.Id,
            status = "aprovado",
            notificacao = "pendente - sera processada em background",
            fluxo = "assincrono",
            tempoTotalMs = tempoTotal,
            observacao = "Resposta liberada apos o pagamento; notificacao processada em segundo plano."
        });
    }
}
