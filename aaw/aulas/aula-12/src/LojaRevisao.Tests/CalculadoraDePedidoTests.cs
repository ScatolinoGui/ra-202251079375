using LojaRevisao.Api.Modulos.Pedidos;
using Xunit;

namespace LojaRevisao.Tests;

public class CalculadoraDePedidoTests
{
    [Theory]
    [InlineData(100.00, 25.00)]
    [InlineData(299.99, 25.00)]
    [InlineData(300.00, 0.00)]
    [InlineData(300.01, 0.00)]
    public void CalcularFrete_ConformeSubtotal_RetornaFreteDaRegra(double subtotal, double freteEsperado)
    {
        var frete = CalculadoraDePedido.CalcularFrete((decimal)subtotal);

        Assert.Equal((decimal)freteEsperado, frete);
    }

    [Fact]
    public void CalcularTotal_CompraPequena_SomaOFrete()
    {
        var total = CalculadoraDePedido.CalcularTotal(100m);

        Assert.Equal(125m, total);
    }
}
