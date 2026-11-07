using LojaRevisao.Api.Data;
using LojaRevisao.Api.Models;
using LojaRevisao.Api.Modulos.Catalogo;
using LojaRevisao.Api.Modulos.Notificacoes;
using Microsoft.EntityFrameworkCore;

namespace LojaRevisao.Api.Modulos.Pedidos;

public class PedidoService
{
    private readonly LojaDbContext _db;
    private readonly ICatalogo _catalogo;
    private readonly FilaDeNotificacoes _fila;

    public PedidoService(LojaDbContext db, ICatalogo catalogo, FilaDeNotificacoes fila)
    {
        _db = db;
        _catalogo = catalogo;
        _fila = fila;
    }

    public async Task<Pedido> CriarAsync(int usuarioId, NovoPedido novo)
    {
        var pedido = new Pedido { UsuarioId = usuarioId };

        foreach (var item in novo.Itens)
        {
            // Usa o contrato ICatalogo para respeitar a fronteira do modulo e validar estoque disponivel.
            var produto = _catalogo.Reservar(item.ProdutoId, item.Quantidade);

            pedido.Itens.Add(new ItemPedido
            {
                ProdutoId = produto.Id,
                Quantidade = item.Quantidade,
                PrecoUnitario = produto.Preco
            });
        }

        pedido.Subtotal = pedido.Itens.Sum(i => i.PrecoUnitario * i.Quantidade);
        pedido.Frete = CalculadoraDePedido.CalcularFrete(pedido.Subtotal);
        pedido.Total = CalculadoraDePedido.CalcularTotal(pedido.Subtotal);

        _db.Pedidos.Add(pedido);
        // Salva reserva de estoque e pedido juntos em uma unica transacao atomica.
        _db.SaveChanges();

        // Publica evento na fila para envio de confirmacao em background sem travar a resposta HTTP.
        await _fila.PublicarAsync(new PedidoCriado(pedido.Id));

        return pedido;
    }

    public Pedido? Obter(int id) =>
        _db.Pedidos.Include(p => p.Itens).FirstOrDefault(p => p.Id == id);

    public List<Pedido> ListarDoUsuario(int usuarioId) =>
        _db.Pedidos.Include(p => p.Itens).Where(p => p.UsuarioId == usuarioId).OrderBy(p => p.Id).ToList();

    /// <summary>Cancela o pedido e devolve as unidades ao estoque.</summary>
    public void Cancelar(Pedido pedido)
    {
        foreach (var item in pedido.Itens)
            _catalogo.Devolver(item.ProdutoId, item.Quantidade);

        _db.Pedidos.Remove(pedido);
        _db.SaveChanges();
    }
}
