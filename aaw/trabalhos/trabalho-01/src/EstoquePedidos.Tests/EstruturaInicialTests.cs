namespace EstoquePedidos.Tests;

public class EstruturaInicialTests
{
    [Fact]
    public void ReferenciaDaApi_DeveEstarAcessivelNoProjetoDeTestes()
    {
        var tipoProgram = typeof(Program);

        Assert.NotNull(tipoProgram);
    }
}
