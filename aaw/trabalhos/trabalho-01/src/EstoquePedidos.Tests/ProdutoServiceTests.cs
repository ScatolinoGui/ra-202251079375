using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Exceptions;
using EstoquePedidos.Api.Models;
using EstoquePedidos.Api.Services;
using EstoquePedidos.Tests.Fakes;

namespace EstoquePedidos.Tests;

public class ProdutoServiceTests
{
    private static (ProdutoService Service, FakeProdutoRepository ProdutoRepo, FakePedidoRepository PedidoRepo) CriarSut()
    {
        var produtoRepo = new FakeProdutoRepository();
        var pedidoRepo = new FakePedidoRepository();
        var service = new ProdutoService(produtoRepo, pedidoRepo);
        return (service, produtoRepo, pedidoRepo);
    }

    // Teste 1: Criar produto valido salva com sucesso e retorna ProdutoResponse
    [Fact]
    public async Task CriarAsync_ProdutoValido_DeveSalvarERetornarProdutoResponse()
    {
        var (service, produtoRepo, _) = CriarSut();
        var request = new CriarProdutoRequest(
            Nome: "Teclado Mecanico Compacto",
            Categoria: "Perifericos",
            Preco: 280.00m,
            Estoque: 15);

        var resposta = await service.CriarAsync(request);

        Assert.NotNull(resposta);
        Assert.False(string.IsNullOrWhiteSpace(resposta.Id));
        Assert.Equal("Teclado Mecanico Compacto", resposta.Nome);
        Assert.Equal("Perifericos", resposta.Categoria);
        Assert.Equal(280.00m, resposta.Preco);
        Assert.Equal(15, resposta.Estoque);
        Assert.True(resposta.Ativo);
        Assert.Equal(1, await produtoRepo.ContarAsync());
    }

    // Teste 2: Criar produto com preco zero ou negativo lanca ArgumentException (400)
    [Fact]
    public async Task CriarAsync_PrecoZeroOuNegativo_DeveLancarArgumentException()
    {
        var (service, produtoRepo, _) = CriarSut();
        var requestPrecoZero = new CriarProdutoRequest("Mouse USB", "Perifericos", 0m, 10);
        var requestPrecoNegativo = new CriarProdutoRequest("Mouse USB", "Perifericos", -5.00m, 10);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(requestPrecoZero));
        await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(requestPrecoNegativo));
        Assert.Equal(0, await produtoRepo.ContarAsync());
    }

    // Teste 3: Criar produto com nome duplicado lanca ConflitoException (409)
    [Fact]
    public async Task CriarAsync_NomeDuplicado_DeveLancarConflitoException()
    {
        var (service, produtoRepo, _) = CriarSut();
        await service.CriarAsync(new CriarProdutoRequest("Monitor IPS 24", "Monitores", 890.00m, 8));

        var requestDuplicado = new CriarProdutoRequest("monitor ips 24", "Monitores", 920.00m, 5);

        await Assert.ThrowsAsync<ConflitoException>(() => service.CriarAsync(requestDuplicado));
        Assert.Equal(1, await produtoRepo.ContarAsync());
    }

    // Teste 4: Excluir produto que ja possui pedidos vinculados lanca ConflitoException (409)
    [Fact]
    public async Task RemoverAsync_ProdutoComPedidosVinculados_DeveLancarConflitoException()
    {
        var (service, produtoRepo, pedidoRepo) = CriarSut();
        var produto = await service.CriarAsync(
            new CriarProdutoRequest("Headset USB", "Audio", 190.00m, 10));

        await pedidoRepo.CriarAsync(new Pedido
        {
            ProdutoId = produto.Id,
            NomeProduto = produto.Nome,
            NomeCliente = "Cliente Teste",
            Quantidade = 1,
            Subtotal = 190.00m,
            Desconto = 0m,
            Frete = 0m,
            ValorTotal = 190.00m,
            Status = "Confirmado"
        });

        await Assert.ThrowsAsync<ConflitoException>(() => service.RemoverAsync(produto.Id));
        Assert.Equal(1, await produtoRepo.ContarAsync());
    }
}
