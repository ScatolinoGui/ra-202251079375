using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Exceptions;
using EstoquePedidos.Api.Models;
using EstoquePedidos.Api.Services;
using EstoquePedidos.Tests.Fakes;

namespace EstoquePedidos.Tests;

public class PedidoServiceTests
{
    private static (PedidoService Service, FakeProdutoRepository ProdutoRepo, FakePedidoRepository PedidoRepo) CriarSut()
    {
        var produtoRepo = new FakeProdutoRepository();
        var pedidoRepo = new FakePedidoRepository();
        var service = new PedidoService(pedidoRepo, produtoRepo);
        return (service, produtoRepo, pedidoRepo);
    }

    // Teste 5: Criar pedido com estoque suficiente desconta o estoque do produto e calcula subtotal, desconto, frete e total
    [Fact]
    public async Task CriarAsync_ComEstoqueSuficiente_DeveBaixarEstoqueECalcularValores()
    {
        var (service, produtoRepo, _) = CriarSut();
        var produto = await produtoRepo.CriarAsync(new Produto
        {
            Nome = "Teclado Mecanico Compacto",
            Categoria = "Perifericos",
            Preco = 200.00m,
            Estoque = 10,
            Ativo = true
        });

        var request = new CriarPedidoRequest(
            ProdutoId: produto.Id,
            NomeCliente: "Mariana Costa",
            Quantidade: 2,
            Cupom: "DESC10");

        var pedido = await service.CriarAsync(request);
        var produtoAtualizado = await produtoRepo.ObterPorIdAsync(produto.Id);

        Assert.Equal(400.00m, pedido.Subtotal);
        Assert.Equal(40.00m, pedido.Desconto);
        Assert.Equal(0.00m, pedido.Frete);
        Assert.Equal(360.00m, pedido.ValorTotal);
        Assert.Equal("Confirmado", pedido.Status);
        Assert.NotNull(produtoAtualizado);
        Assert.Equal(8, produtoAtualizado!.Estoque);
    }

    // Teste 6: Criar pedido com quantidade maior que o estoque disponivel lanca ConflitoException (409)
    [Fact]
    public async Task CriarAsync_QuantidadeMaiorQueEstoque_DeveLancarConflitoException()
    {
        var (service, produtoRepo, _) = CriarSut();
        var produto = await produtoRepo.CriarAsync(new Produto
        {
            Nome = "Monitor IPS 24 Polegadas",
            Categoria = "Monitores",
            Preco = 890.00m,
            Estoque = 3,
            Ativo = true
        });

        var request = new CriarPedidoRequest(
            ProdutoId: produto.Id,
            NomeCliente: "Rafael Almeida",
            Quantidade: 4,
            Cupom: null);

        await Assert.ThrowsAsync<ConflitoException>(() => service.CriarAsync(request));

        var produtoIntacto = await produtoRepo.ObterPorIdAsync(produto.Id);
        Assert.Equal(3, produtoIntacto!.Estoque);
    }

    // Teste 7: Calcular frete retorna 0m exatamente na fronteira de 100.00m e acima (150.00m), e retorna 15.00m abaixo (99.99m)
    [Theory]
    [InlineData(99.99, 15.00)]
    [InlineData(100.00, 0.00)]
    [InlineData(150.00, 0.00)]
    public void CalcularFrete_ValoresDeFronteira_DeveRetornarFreteCorreto(
        decimal valorComDesconto,
        decimal freteEsperado)
    {
        var (service, _, _) = CriarSut();

        var freteCalculado = service.CalcularFrete(valorComDesconto);

        Assert.Equal(freteEsperado, freteCalculado);
    }

    // Teste 8: Calcular desconto aplica 10% com cupom "DESC10" ou quando quantidade >= 10, e 0m sem cupom
    [Theory]
    [InlineData(200.00, 2, null, 0.00)]
    [InlineData(200.00, 2, "DESC10", 20.00)]
    [InlineData(200.00, 10, null, 20.00)]
    [InlineData(200.00, 12, "desc10", 20.00)]
    public void CalcularDesconto_ComCupomOuQuantidadeAtacado_DeveAplicarDezPorCentoSemAcumular(
        decimal subtotal,
        int quantidade,
        string? cupom,
        decimal descontoEsperado)
    {
        var (service, _, _) = CriarSut();

        var descontoCalculado = service.CalcularDesconto(subtotal, quantidade, cupom);

        Assert.Equal(descontoEsperado, descontoCalculado);
    }

    // Teste 9: Cancelar pedido confirmado devolve a quantidade ao estoque do produto e altera o status para "Cancelado"
    [Fact]
    public async Task CancelarAsync_PedidoConfirmado_DeveEstornarEstoqueEMudarStatusParaCancelado()
    {
        var (service, produtoRepo, _) = CriarSut();
        var produto = await produtoRepo.CriarAsync(new Produto
        {
            Nome = "Cabo HDMI 2.1 Ultra HD",
            Categoria = "Cabos",
            Preco = 45.00m,
            Estoque = 20,
            Ativo = true
        });

        var criado = await service.CriarAsync(new CriarPedidoRequest(
            ProdutoId: produto.Id,
            NomeCliente: "Lucas Ferreira",
            Quantidade: 3,
            Cupom: null));

        var aposCompra = await produtoRepo.ObterPorIdAsync(produto.Id);
        Assert.Equal(17, aposCompra!.Estoque);

        var cancelado = await service.CancelarAsync(criado.Id);
        var aposCancelamento = await produtoRepo.ObterPorIdAsync(produto.Id);

        Assert.Equal("Cancelado", cancelado.Status);
        Assert.Equal(20, aposCancelamento!.Estoque);
    }
}
