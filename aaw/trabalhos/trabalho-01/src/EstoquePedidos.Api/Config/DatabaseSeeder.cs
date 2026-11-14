using EstoquePedidos.Api.Models;
using EstoquePedidos.Api.Repositories;
using EstoquePedidos.Api.Services;

namespace EstoquePedidos.Api.Config;

public class DatabaseSeeder
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        IUsuarioRepository usuarioRepository,
        IProdutoRepository produtoRepository,
        IPedidoRepository pedidoRepository,
        ILogger<DatabaseSeeder> logger)
    {
        _usuarioRepository = usuarioRepository;
        _produtoRepository = produtoRepository;
        _pedidoRepository = pedidoRepository;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedUsuariosAsync();
            await SeedCatalogoEPedidosAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Nao foi possivel executar o seed inicial do MongoDB. Verifique se o banco esta ativo.");
        }
    }

    private async Task SeedUsuariosAsync()
    {
        if (await _usuarioRepository.ContarUsuariosAsync() > 0)
        {
            return;
        }

        var admin = new Usuario
        {
            Nome = "Administrador",
            Email = "admin@estoquepedidos.com",
            SenhaHash = AuthService.GerarHashSenha("Admin@123"),
            Role = "Admin"
        };

        var cliente = new Usuario
        {
            Nome = "Cliente Teste",
            Email = "cliente@estoquepedidos.com",
            SenhaHash = AuthService.GerarHashSenha("Cliente@123"),
            Role = "Usuario"
        };

        await _usuarioRepository.CriarAsync(admin);
        await _usuarioRepository.CriarAsync(cliente);

        _logger.LogInformation("Usuarios iniciais criados com sucesso (Admin e Usuario).");
    }

    private async Task SeedCatalogoEPedidosAsync()
    {
        if (await _produtoRepository.ContarAsync() > 0)
        {
            return;
        }

        var produtosIniciais = new List<Produto>
        {
            new()
            {
                Nome = "Teclado Mecanico Compacto",
                Categoria = "Perifericos",
                Preco = 280.00m,
                Estoque = 25,
                Ativo = true,
                CriadoEm = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                Nome = "Monitor IPS 24 Polegadas",
                Categoria = "Monitores",
                Preco = 890.00m,
                Estoque = 12,
                Ativo = true,
                CriadoEm = DateTime.UtcNow.AddDays(-9)
            },
            new()
            {
                Nome = "Mouse Optico Sem Fio",
                Categoria = "Perifericos",
                Preco = 95.00m,
                Estoque = 40,
                Ativo = true,
                CriadoEm = DateTime.UtcNow.AddDays(-8)
            },
            new()
            {
                Nome = "Headset USB com Microfone",
                Categoria = "Audio",
                Preco = 190.00m,
                Estoque = 18,
                Ativo = true,
                CriadoEm = DateTime.UtcNow.AddDays(-7)
            },
            new()
            {
                Nome = "Suporte Articulado de Notebook",
                Categoria = "Acessorios",
                Preco = 135.00m,
                Estoque = 30,
                Ativo = true,
                CriadoEm = DateTime.UtcNow.AddDays(-6)
            },
            new()
            {
                Nome = "Cabo HDMI 2.1 Ultra HD",
                Categoria = "Cabos",
                Preco = 45.00m,
                Estoque = 60,
                Ativo = true,
                CriadoEm = DateTime.UtcNow.AddDays(-5)
            }
        };

        foreach (var produto in produtosIniciais)
        {
            await _produtoRepository.CriarAsync(produto);
        }

        var teclado = produtosIniciais[0];
        var monitor = produtosIniciais[1];
        var cabo = produtosIniciais[5];

        var pedidosIniciais = new List<Pedido>
        {
            new()
            {
                ProdutoId = teclado.Id,
                NomeProduto = teclado.Nome,
                NomeCliente = "Mariana Costa",
                Quantidade = 2,
                Cupom = null,
                Subtotal = 560.00m,
                Desconto = 0.00m,
                Frete = 0.00m,
                ValorTotal = 560.00m,
                Status = "Confirmado",
                CriadoEm = DateTime.UtcNow.AddDays(-3)
            },
            new()
            {
                ProdutoId = monitor.Id,
                NomeProduto = monitor.Nome,
                NomeCliente = "Rafael Almeida",
                Quantidade = 1,
                Cupom = "DESC10",
                Subtotal = 890.00m,
                Desconto = 89.00m,
                Frete = 0.00m,
                ValorTotal = 801.00m,
                Status = "Confirmado",
                CriadoEm = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                ProdutoId = cabo.Id,
                NomeProduto = cabo.Nome,
                NomeCliente = "Lucas Ferreira",
                Quantidade = 1,
                Cupom = null,
                Subtotal = 45.00m,
                Desconto = 0.00m,
                Frete = 15.00m,
                ValorTotal = 60.00m,
                Status = "Confirmado",
                CriadoEm = DateTime.UtcNow.AddDays(-1)
            }
        };

        foreach (var pedido in pedidosIniciais)
        {
            await _pedidoRepository.CriarAsync(pedido);
        }

        _logger.LogInformation("Catalogo inicial (6 produtos e 3 pedidos) inserido no MongoDB.");
    }
}
