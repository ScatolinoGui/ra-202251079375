const STORAGE_KEY_AUTH = "estoque_pedidos_auth";

const estadoApp = {
    auth: carregarSessaoLocal(),
    produtosPagina: 1,
    produtosTamanhoPagina: 6,
    produtosTotalPaginas: 1,
    pedidosPagina: 1,
    pedidosTamanhoPagina: 10,
    pedidosTotalPaginas: 1,
    catalogoCache: [],
    produtoDetalheAtual: null
};

document.addEventListener("DOMContentLoaded", () => {
    configurarAbas();
    configurarAutenticacao();
    configurarCatalogoEPedidos();
    configurarVisaoDetalhe();
    configurarFormularios();

    atualizarPainelAutenticacao();
    carregarOpcoesProdutosSelects();
    carregarProdutos();
    carregarPedidos();
});

// Persistencia local da sessao JWT
function carregarSessaoLocal() {
    try {
        const bruto = localStorage.getItem(STORAGE_KEY_AUTH);
        if (!bruto) {
            return null;
        }
        const dados = JSON.parse(bruto);
        if (!dados || !dados.token) {
            return null;
        }
        return dados;
    } catch {
        return null;
    }
}

function salvarSessaoLocal(dadosAuth) {
    estadoApp.auth = {
        token: dadosAuth.token,
        nome: dadosAuth.nome,
        email: dadosAuth.email,
        role: dadosAuth.role
    };
    localStorage.setItem(STORAGE_KEY_AUTH, JSON.stringify(estadoApp.auth));
    atualizarPainelAutenticacao();
}

function limparSessaoLocal() {
    estadoApp.auth = null;
    localStorage.removeItem(STORAGE_KEY_AUTH);
    atualizarPainelAutenticacao();
}

// Cliente HTTP centralizado com suporte a Bearer token e leitura de ProblemDetails (RFC 9457)
async function requisitarApi(url, opcoes = {}) {
    const headers = {
        Accept: "application/json",
        ...(opcoes.headers || {})
    };

    if (opcoes.body && !headers["Content-Type"]) {
        headers["Content-Type"] = "application/json";
    }

    if (estadoApp.auth && estadoApp.auth.token) {
        headers.Authorization = `Bearer ${estadoApp.auth.token}`;
    }

    let resposta;
    try {
        resposta = await fetch(url, {
            ...opcoes,
            headers
        });
    } catch {
        throw new Error("Falha de comunicacao com o servidor. Verifique se a API esta em execucao.");
    }

    if (resposta.status === 204) {
        return null;
    }

    let corpo = null;
    const texto = await resposta.text();
    if (texto) {
        try {
            corpo = JSON.parse(texto);
        } catch {
            corpo = null;
        }
    }

    if (!resposta.ok) {
        const detalhe =
            (corpo && (corpo.detail || corpo.title || corpo.mensagem)) ||
            `Erro HTTP ${resposta.status} ao processar a requisicao.`;
        const erro = new Error(detalhe);
        erro.status = resposta.status;
        erro.problem = corpo;
        throw erro;
    }

    return corpo;
}

// Controle explicito dos 4 estados de UI: loading, error, empty e success
function definirEstadoContainer(elementoId, tipo, mensagem, opcoesAcao = null) {
    const el = document.getElementById(elementoId);
    if (!el) {
        return;
    }

    el.className = "estado-ui";
    el.innerHTML = "";

    if (!tipo || tipo === "oculto") {
        el.classList.add("oculto");
        el.setAttribute("role", "status");
        return;
    }

    el.classList.add(`estado-${tipo}`);
    el.setAttribute("role", tipo === "error" ? "alert" : "status");

    const spanTexto = document.createElement("span");
    spanTexto.textContent = mensagem;
    el.appendChild(spanTexto);

    if (opcoesAcao && typeof opcoesAcao.callback === "function") {
        const btn = document.createElement("button");
        btn.type = "button";
        btn.className = "btn btn-secundario btn-pequeno";
        btn.style.marginLeft = "12px";
        btn.textContent = opcoesAcao.rotulo || "Tentar novamente";
        btn.addEventListener("click", opcoesAcao.callback);
        el.appendChild(btn);
    }
}

function exibirAlertaGlobal(tipo, mensagem) {
    definirEstadoContainer("alerta-global", tipo, mensagem, {
        rotulo: "Fechar aviso",
        callback: () => definirEstadoContainer("alerta-global", "oculto", "")
    });
}

// Navegacao entre as 3 visoes obrigatorias
function configurarAbas() {
    const botoesAbas = document.querySelectorAll(".aba-btn");
    botoesAbas.forEach((botao) => {
        botao.addEventListener("click", () => {
            ativarVisao(botao.dataset.target);
        });
    });

    document.getElementById("btn-ir-novo-produto").addEventListener("click", () => {
        prepararFormularioNovoProduto();
        ativarVisao("visao-formulario");
        document.getElementById("produto-nome").focus();
    });

    document.getElementById("btn-ir-novo-pedido").addEventListener("click", () => {
        ativarVisao("visao-formulario");
        document.getElementById("pedido-produto-id").focus();
    });

    document.getElementById("btn-voltar-catalogo").addEventListener("click", () => {
        ativarVisao("visao-lista");
    });
}

function ativarVisao(visaoId) {
    document.querySelectorAll(".visao").forEach((secao) => {
        if (secao.id === visaoId) {
            secao.classList.remove("oculto");
            secao.classList.add("ativa");
        } else {
            secao.classList.add("oculto");
            secao.classList.remove("ativa");
        }
    });

    document.querySelectorAll(".aba-btn").forEach((aba) => {
        const ativa = aba.dataset.target === visaoId;
        aba.classList.toggle("ativa", ativa);
        aba.setAttribute("aria-selected", ativa ? "true" : "false");
    });
}

// Painel de login JWT e botoes de preenchimento rapido
function configurarAutenticacao() {
    const formLogin = document.getElementById("form-login");
    const inputEmail = document.getElementById("login-email");
    const inputSenha = document.getElementById("login-senha");

    document.getElementById("btn-fill-admin").addEventListener("click", () => {
        inputEmail.value = "admin@estoquepedidos.com";
        inputSenha.value = "Admin@123";
    });

    document.getElementById("btn-fill-cliente").addEventListener("click", () => {
        inputEmail.value = "cliente@estoquepedidos.com";
        inputSenha.value = "Cliente@123";
    });

    formLogin.addEventListener("submit", async (evento) => {
        evento.preventDefault();
        const email = inputEmail.value.trim();
        const senha = inputSenha.value;

        definirEstadoContainer("alerta-global", "loading", "Carregando dados de autenticacao...");

        try {
            const resposta = await requisitarApi("/api/auth/login", {
                method: "POST",
                body: JSON.stringify({ email, senha })
            });

            salvarSessaoLocal(resposta);
            inputSenha.value = "";
            exibirAlertaGlobal(
                "success",
                `Sessao iniciada como ${resposta.nome} (perfil ${resposta.role}).`
            );
            await carregarPedidos();
        } catch (erro) {
            definirEstadoContainer("alerta-global", "error", erro.message, {
                rotulo: "Fechar",
                callback: () => definirEstadoContainer("alerta-global", "oculto", "")
            });
        }
    });

    document.getElementById("btn-logout").addEventListener("click", () => {
        limparSessaoLocal();
        exibirAlertaGlobal("empty", "Sessao encerrada. Rotas protegidas exigirao novo login.");
        carregarPedidos();
    });
}

function atualizarPainelAutenticacao() {
    const formLogin = document.getElementById("form-login");
    const boxLogado = document.getElementById("auth-status-logado");

    if (estadoApp.auth && estadoApp.auth.token) {
        formLogin.classList.add("oculto");
        boxLogado.classList.remove("oculto");
        document.getElementById("auth-usuario-nome").textContent = estadoApp.auth.nome;
        document.getElementById("auth-usuario-email").textContent = `(${estadoApp.auth.email})`;
        document.getElementById("auth-usuario-role").textContent = estadoApp.auth.role;
    } else {
        formLogin.classList.remove("oculto");
        boxLogado.classList.add("oculto");
    }
}

// VISAO 1: Catalogo de produtos e listagem geral de pedidos
function configurarCatalogoEPedidos() {
    const formFiltro = document.getElementById("form-filtro-produtos");
    formFiltro.addEventListener("submit", (evento) => {
        evento.preventDefault();
        estadoApp.produtosPagina = 1;
        estadoApp.produtosTamanhoPagina = Number(document.getElementById("filtro-pagesize").value) || 6;
        carregarProdutos();
    });

    document.getElementById("btn-limpar-filtros").addEventListener("click", () => {
        limparFiltrosProdutos();
    });

    document.getElementById("btn-pagina-anterior").addEventListener("click", () => {
        if (estadoApp.produtosPagina > 1) {
            estadoApp.produtosPagina -= 1;
            carregarProdutos();
        }
    });

    document.getElementById("btn-proxima-pagina").addEventListener("click", () => {
        if (estadoApp.produtosPagina < estadoApp.produtosTotalPaginas) {
            estadoApp.produtosPagina += 1;
            carregarProdutos();
        }
    });

    document.getElementById("filtro-status-pedido").addEventListener("change", () => {
        estadoApp.pedidosPagina = 1;
        carregarPedidos();
    });

    document.getElementById("btn-atualizar-pedidos").addEventListener("click", () => {
        carregarPedidos();
    });

    document.getElementById("btn-pedidos-anterior").addEventListener("click", () => {
        if (estadoApp.pedidosPagina > 1) {
            estadoApp.pedidosPagina -= 1;
            carregarPedidos();
        }
    });

    document.getElementById("btn-pedidos-proxima").addEventListener("click", () => {
        if (estadoApp.pedidosPagina < estadoApp.pedidosTotalPaginas) {
            estadoApp.pedidosPagina += 1;
            carregarPedidos();
        }
    });
}

function limparFiltrosProdutos() {
    document.getElementById("filtro-categoria").value = "";
    document.getElementById("filtro-busca").value = "";
    document.getElementById("filtro-pagesize").value = "6";
    estadoApp.produtosPagina = 1;
    estadoApp.produtosTamanhoPagina = 6;
    carregarProdutos();
}

async function carregarProdutos() {
    const containerLista = document.getElementById("lista-produtos");
    const categoria = document.getElementById("filtro-categoria").value.trim();
    const busca = document.getElementById("filtro-busca").value.trim();
    const page = estadoApp.produtosPagina;
    const pageSize = estadoApp.produtosTamanhoPagina;

    containerLista.innerHTML = "";
    definirEstadoContainer("estado-produtos", "loading", "Carregando dados...");

    const params = new URLSearchParams({
        page: String(page),
        pageSize: String(pageSize)
    });
    if (categoria) {
        params.set("categoria", categoria);
    }
    if (busca) {
        params.set("busca", busca);
    }

    try {
        const resultado = await requisitarApi(`/api/produtos?${params.toString()}`);
        const itens = resultado.items || [];
        const total = resultado.total || 0;
        const totalPaginas = Math.max(1, Math.ceil(total / pageSize));
        estadoApp.produtosTotalPaginas = totalPaginas;

        atualizarPaginacaoProdutos(page, totalPaginas, total);

        if (itens.length === 0) {
            definirEstadoContainer(
                "estado-produtos",
                "empty",
                "Nenhum registro encontrado para os filtros informados.",
                {
                    rotulo: "Limpar filtros",
                    callback: limparFiltrosProdutos
                }
            );
            return;
        }

        definirEstadoContainer(
            "estado-produtos",
            "success",
            `Exibindo ${itens.length} de ${total} produto(s) cadastrado(s).`
        );
        renderizarCardsProdutos(itens);
    } catch (erro) {
        definirEstadoContainer("estado-produtos", "error", erro.message, {
            rotulo: "Tentar novamente",
            callback: carregarProdutos
        });
    }
}

function atualizarPaginacaoProdutos(paginaAtual, totalPaginas, totalItens) {
    document.getElementById("info-paginacao-produtos").textContent =
        `Pagina ${paginaAtual} de ${totalPaginas} (${totalItens} registro(s))`;
    document.getElementById("btn-pagina-anterior").disabled = paginaAtual <= 1;
    document.getElementById("btn-proxima-pagina").disabled = paginaAtual >= totalPaginas;
}

function renderizarCardsProdutos(produtos) {
    const container = document.getElementById("lista-produtos");
    container.innerHTML = "";

    produtos.forEach((produto) => {
        const card = document.createElement("article");
        card.className = "card-produto";

        const classeStatus = produto.ativo ? "etiqueta-status-ativo" : "etiqueta-status-inativo";
        const textoStatus = produto.ativo ? "Ativo" : "Inativo";

        card.innerHTML = `
            <div>
                <div class="card-produto-topo">
                    <span class="etiqueta-simples">${escaparHtml(produto.categoria)}</span>
                    <span class="etiqueta-simples ${classeStatus}">${textoStatus}</span>
                </div>
                <h3>${escaparHtml(produto.nome)}</h3>
            </div>
            <div>
                <div class="card-produto-dados">
                    <span class="card-preco">${formatarMoeda(produto.preco)}</span>
                    <span class="card-estoque">Estoque: <strong>${produto.estoque}</strong> un.</span>
                </div>
                <div class="card-acoes" style="margin-top: 12px;">
                    <button type="button" class="btn btn-primario btn-pequeno btn-ver-detalhe">Ver detalhes e pedidos</button>
                    <button type="button" class="btn btn-secundario btn-pequeno btn-comprar-card">Emitir pedido</button>
                    <button type="button" class="btn btn-secundario btn-pequeno btn-editar-card">Editar</button>
                </div>
            </div>
        `;

        card.querySelector(".btn-ver-detalhe").addEventListener("click", () => {
            abrirDetalheProduto(produto.id);
        });

        card.querySelector(".btn-comprar-card").addEventListener("click", () => {
            selecionarProdutoParaPedido(produto.id);
        });

        card.querySelector(".btn-editar-card").addEventListener("click", () => {
            preencherFormularioEdicaoProduto(produto);
            ativarVisao("visao-formulario");
        });

        container.appendChild(card);
    });
}

async function carregarOpcoesProdutosSelects() {
    try {
        const resultado = await requisitarApi("/api/produtos?page=1&pageSize=100");
        const itens = resultado.items || [];
        estadoApp.catalogoCache = itens;

        const selectDetalhe = document.getElementById("select-produto-detalhe");
        const selectPedido = document.getElementById("pedido-produto-id");

        const valorDetalheAtual = selectDetalhe.value;
        const valorPedidoAtual = selectPedido.value;

        selectDetalhe.innerHTML = '<option value="">Selecione um produto...</option>';
        selectPedido.innerHTML = '<option value="">Selecione um produto disponivel...</option>';

        itens.forEach((produto) => {
            const optDetalhe = document.createElement("option");
            optDetalhe.value = produto.id;
            optDetalhe.textContent = `${produto.nome} (${produto.categoria})`;
            selectDetalhe.appendChild(optDetalhe);

            const optPedido = document.createElement("option");
            optPedido.value = produto.id;
            optPedido.textContent = `${produto.nome} - ${formatarMoeda(produto.preco)} (Estoque: ${produto.estoque})`;
            selectPedido.appendChild(optPedido);
        });

        if (valorDetalheAtual) {
            selectDetalhe.value = valorDetalheAtual;
        }
        if (valorPedidoAtual) {
            selectPedido.value = valorPedidoAtual;
        }

        atualizarPreviaPedido();
    } catch {
        // Silencioso no pre-carregamento; o erro principal ja aparece no estado do catalogo
    }
}

async function carregarPedidos() {
    const wrapperTabela = document.getElementById("container-tabela-pedidos");
    const barraPaginacao = document.getElementById("paginacao-pedidos");
    const tbody = document.getElementById("lista-pedidos");
    const statusFiltro = document.getElementById("filtro-status-pedido").value.trim();

    tbody.innerHTML = "";
    wrapperTabela.classList.add("oculto");
    barraPaginacao.classList.add("oculto");

    definirEstadoContainer("estado-pedidos", "loading", "Carregando dados...");

    const params = new URLSearchParams({
        page: String(estadoApp.pedidosPagina),
        pageSize: String(estadoApp.pedidosTamanhoPagina)
    });
    if (statusFiltro) {
        params.set("status", statusFiltro);
    }

    try {
        const resultado = await requisitarApi(`/api/pedidos?${params.toString()}`);
        const itens = resultado.items || [];
        const total = resultado.total || 0;
        const totalPaginas = Math.max(1, Math.ceil(total / estadoApp.pedidosTamanhoPagina));
        estadoApp.pedidosTotalPaginas = totalPaginas;

        if (itens.length === 0) {
            definirEstadoContainer(
                "estado-pedidos",
                "empty",
                "Nenhum registro encontrado na listagem de pedidos.",
                statusFiltro
                    ? {
                          rotulo: "Mostrar todos os status",
                          callback: () => {
                              document.getElementById("filtro-status-pedido").value = "";
                              estadoApp.pedidosPagina = 1;
                              carregarPedidos();
                          }
                      }
                    : null
            );
            return;
        }

        definirEstadoContainer(
            "estado-pedidos",
            "success",
            `Exibindo ${itens.length} de ${total} pedido(s).`
        );
        renderizarTabelaPedidos(itens);
        wrapperTabela.classList.remove("oculto");
        barraPaginacao.classList.remove("oculto");

        document.getElementById("info-paginacao-pedidos").textContent =
            `Pagina ${estadoApp.pedidosPagina} de ${totalPaginas} (${total} pedido(s))`;
        document.getElementById("btn-pedidos-anterior").disabled = estadoApp.pedidosPagina <= 1;
        document.getElementById("btn-pedidos-proxima").disabled = estadoApp.pedidosPagina >= totalPaginas;
    } catch (erro) {
        definirEstadoContainer("estado-pedidos", "error", erro.message, {
            rotulo: "Tentar novamente",
            callback: carregarPedidos
        });
    }
}

function renderizarTabelaPedidos(pedidos) {
    const tbody = document.getElementById("lista-pedidos");
    tbody.innerHTML = "";

    pedidos.forEach((pedido) => {
        const tr = document.createElement("tr");
        const cancelado = String(pedido.status).toLowerCase() === "cancelado";
        const classeStatus = cancelado ? "etiqueta-status-cancelado" : "etiqueta-status-ativo";

        tr.innerHTML = `
            <td>${escaparHtml(pedido.nomeCliente)}</td>
            <td>
                <button type="button" class="btn-link btn-abrir-produto" style="color: #1e3a8a;">
                    ${escaparHtml(pedido.nomeProduto)}
                </button>
            </td>
            <td>${pedido.quantidade}</td>
            <td>${pedido.cupom ? `<code>${escaparHtml(pedido.cupom)}</code>` : "-"}</td>
            <td>${formatarMoeda(pedido.subtotal)}</td>
            <td>${formatarMoeda(pedido.desconto)}</td>
            <td>${formatarMoeda(pedido.frete)}</td>
            <td><strong>${formatarMoeda(pedido.valorTotal)}</strong></td>
            <td><span class="etiqueta-simples ${classeStatus}">${escaparHtml(pedido.status)}</span></td>
            <td>${formatarData(pedido.criadoEm)}</td>
            <td>
                ${
                    cancelado
                        ? '<span class="texto-auxiliar">Estornado</span>'
                        : '<button type="button" class="btn btn-perigo btn-pequeno btn-cancelar-pedido">Cancelar</button>'
                }
            </td>
        `;

        tr.querySelector(".btn-abrir-produto").addEventListener("click", () => {
            abrirDetalheProduto(pedido.produtoId);
        });

        const btnCancelar = tr.querySelector(".btn-cancelar-pedido");
        if (btnCancelar) {
            btnCancelar.addEventListener("click", () => {
                cancelarPedido(pedido.id);
            });
        }

        tbody.appendChild(tr);
    });
}

async function cancelarPedido(pedidoId) {
    definirEstadoContainer("estado-pedidos", "loading", "Carregando dados do cancelamento...");

    try {
        const cancelado = await requisitarApi(`/api/pedidos/${encodeURIComponent(pedidoId)}`, {
            method: "DELETE"
        });

        exibirAlertaGlobal(
            "success",
            `Pedido de ${cancelado.nomeCliente} cancelado. Estoque de "${cancelado.nomeProduto}" devolvido.`
        );

        await Promise.all([
            carregarProdutos(),
            carregarOpcoesProdutosSelects(),
            carregarPedidos()
        ]);

        if (estadoApp.produtoDetalheAtual && estadoApp.produtoDetalheAtual.id === cancelado.produtoId) {
            await abrirDetalheProduto(cancelado.produtoId, false);
        }
    } catch (erro) {
        definirEstadoContainer("estado-pedidos", "error", erro.message, {
            rotulo: "Recarregar lista",
            callback: carregarPedidos
        });
    }
}

// VISAO 2: Ficha completa do produto e sub-recurso 1-N (GET /api/produtos/{id}/pedidos)
function configurarVisaoDetalhe() {
    document.getElementById("btn-carregar-detalhe").addEventListener("click", () => {
        const id = document.getElementById("select-produto-detalhe").value;
        if (!id) {
            definirEstadoContainer(
                "estado-detalhe-produto",
                "empty",
                "Escolha um produto na caixa de selecao para carregar sua ficha."
            );
            return;
        }
        abrirDetalheProduto(id);
    });

    document.getElementById("btn-pedir-produto-detalhe").addEventListener("click", () => {
        if (estadoApp.produtoDetalheAtual) {
            selecionarProdutoParaPedido(estadoApp.produtoDetalheAtual.id);
        }
    });

    document.getElementById("btn-editar-produto-detalhe").addEventListener("click", () => {
        if (estadoApp.produtoDetalheAtual) {
            preencherFormularioEdicaoProduto(estadoApp.produtoDetalheAtual);
            ativarVisao("visao-formulario");
        }
    });

    document.getElementById("btn-excluir-produto-detalhe").addEventListener("click", () => {
        if (estadoApp.produtoDetalheAtual) {
            excluirProdutoAtual(estadoApp.produtoDetalheAtual.id);
        }
    });
}

async function abrirDetalheProduto(produtoId, mudarAba = true) {
    if (mudarAba) {
        ativarVisao("visao-detalhe");
    }

    document.getElementById("select-produto-detalhe").value = produtoId;
    const cardFicha = document.getElementById("ficha-produto");
    const containerPedidos = document.getElementById("container-pedidos-produto");

    cardFicha.classList.add("oculto");
    containerPedidos.classList.add("oculto");

    definirEstadoContainer("estado-detalhe-produto", "loading", "Carregando dados...");
    definirEstadoContainer("estado-pedidos-produto", "loading", "Carregando dados...");

    try {
        const [produto, pedidosVinculados] = await Promise.all([
            requisitarApi(`/api/produtos/${encodeURIComponent(produtoId)}`),
            requisitarApi(`/api/produtos/${encodeURIComponent(produtoId)}/pedidos`)
        ]);

        estadoApp.produtoDetalheAtual = produto;

        document.getElementById("detalhe-categoria").textContent = produto.categoria;
        document.getElementById("detalhe-nome").textContent = produto.nome;
        document.getElementById("detalhe-id").textContent = produto.id;
        document.getElementById("detalhe-preco").textContent = formatarMoeda(produto.preco);
        document.getElementById("detalhe-estoque").textContent = `${produto.estoque} unidade(s)`;
        document.getElementById("detalhe-ativo").textContent = produto.ativo ? "Ativo" : "Inativo";
        document.getElementById("detalhe-criado-em").textContent = formatarData(produto.criadoEm);

        cardFicha.classList.remove("oculto");
        definirEstadoContainer(
            "estado-detalhe-produto",
            "success",
            `Ficha do produto "${produto.nome}" carregada com sucesso.`
        );

        if (!pedidosVinculados || pedidosVinculados.length === 0) {
            definirEstadoContainer(
                "estado-pedidos-produto",
                "empty",
                "Nenhum registro encontrado: este produto ainda nao possui pedidos vinculados.",
                {
                    rotulo: "Emitir primeiro pedido",
                    callback: () => selecionarProdutoParaPedido(produto.id)
                }
            );
            return;
        }

        definirEstadoContainer(
            "estado-pedidos-produto",
            "success",
            `Encontrados ${pedidosVinculados.length} pedido(s) vinculados a este produto.`
        );
        renderizarPedidosDoProduto(pedidosVinculados);
        containerPedidos.classList.remove("oculto");
    } catch (erro) {
        definirEstadoContainer("estado-detalhe-produto", "error", erro.message, {
            rotulo: "Tentar novamente",
            callback: () => abrirDetalheProduto(produtoId, false)
        });
        definirEstadoContainer("estado-pedidos-produto", "error", erro.message);
    }
}

function renderizarPedidosDoProduto(pedidos) {
    const tbody = document.getElementById("lista-pedidos-produto");
    tbody.innerHTML = "";

    pedidos.forEach((pedido) => {
        const tr = document.createElement("tr");
        const cancelado = String(pedido.status).toLowerCase() === "cancelado";
        const classeStatus = cancelado ? "etiqueta-status-cancelado" : "etiqueta-status-ativo";

        tr.innerHTML = `
            <td><code>${escaparHtml(pedido.id)}</code></td>
            <td>${escaparHtml(pedido.nomeCliente)}</td>
            <td>${pedido.quantidade}</td>
            <td>${pedido.cupom ? `<code>${escaparHtml(pedido.cupom)}</code>` : "-"}</td>
            <td>${formatarMoeda(pedido.subtotal)}</td>
            <td>${formatarMoeda(pedido.desconto)}</td>
            <td>${formatarMoeda(pedido.frete)}</td>
            <td><strong>${formatarMoeda(pedido.valorTotal)}</strong></td>
            <td><span class="etiqueta-simples ${classeStatus}">${escaparHtml(pedido.status)}</span></td>
            <td>${formatarData(pedido.criadoEm)}</td>
        `;
        tbody.appendChild(tr);
    });
}

async function excluirProdutoAtual(produtoId) {
    definirEstadoContainer("estado-detalhe-produto", "loading", "Carregando dados da exclusao...");

    try {
        await requisitarApi(`/api/produtos/${encodeURIComponent(produtoId)}`, {
            method: "DELETE"
        });

        estadoApp.produtoDetalheAtual = null;
        document.getElementById("ficha-produto").classList.add("oculto");
        document.getElementById("container-pedidos-produto").classList.add("oculto");

        definirEstadoContainer(
            "estado-detalhe-produto",
            "success",
            "Produto removido com sucesso do catalogo."
        );
        definirEstadoContainer("estado-pedidos-produto", "empty", "Nenhum produto selecionado.");

        await Promise.all([carregarProdutos(), carregarOpcoesProdutosSelects()]);
    } catch (erro) {
        definirEstadoContainer("estado-detalhe-produto", "error", erro.message, {
            rotulo: "Tentar novamente",
            callback: () => excluirProdutoAtual(produtoId)
        });
    }
}

// VISAO 3: Formularios de cadastro/edicao de produto e emissao de pedido
function configurarFormularios() {
    const formProduto = document.getElementById("form-produto");
    const formPedido = document.getElementById("form-pedido");

    document.getElementById("btn-cancelar-edicao-produto").addEventListener("click", () => {
        prepararFormularioNovoProduto();
    });

    formProduto.addEventListener("submit", async (evento) => {
        evento.preventDefault();
        await salvarProdutoFormulario();
    });

    document.getElementById("pedido-produto-id").addEventListener("change", atualizarPreviaPedido);
    document.getElementById("pedido-quantidade").addEventListener("input", atualizarPreviaPedido);
    document.getElementById("pedido-cupom").addEventListener("input", atualizarPreviaPedido);

    formPedido.addEventListener("submit", async (evento) => {
        evento.preventDefault();
        await emitirPedidoFormulario();
    });
}

function prepararFormularioNovoProduto() {
    document.getElementById("produto-id-edicao").value = "";
    document.getElementById("titulo-form-produto").textContent = "Cadastrar Produto";
    document.getElementById("form-produto").reset();
    document.getElementById("produto-ativo").checked = true;
    document.getElementById("btn-cancelar-edicao-produto").classList.add("oculto");
    definirEstadoContainer("estado-form-produto", "oculto", "");
}

function preencherFormularioEdicaoProduto(produto) {
    document.getElementById("produto-id-edicao").value = produto.id;
    document.getElementById("titulo-form-produto").textContent = `Editar Produto: ${produto.nome}`;
    document.getElementById("produto-nome").value = produto.nome;
    document.getElementById("produto-categoria").value = produto.categoria;
    document.getElementById("produto-preco").value = produto.preco;
    document.getElementById("produto-estoque").value = produto.estoque;
    document.getElementById("produto-ativo").checked = Boolean(produto.ativo);
    document.getElementById("btn-cancelar-edicao-produto").classList.remove("oculto");
    definirEstadoContainer(
        "estado-form-produto",
        "empty",
        `Editando o registro "${produto.nome}". Altere os campos desejados e clique em Salvar produto.`
    );
}

async function salvarProdutoFormulario() {
    const idEdicao = document.getElementById("produto-id-edicao").value.trim();
    const nome = document.getElementById("produto-nome").value.trim();
    const categoria = document.getElementById("produto-categoria").value.trim();
    const preco = Number(document.getElementById("produto-preco").value);
    const estoque = Number(document.getElementById("produto-estoque").value);
    const ativo = document.getElementById("produto-ativo").checked;

    definirEstadoContainer("estado-form-produto", "loading", "Carregando dados...");

    try {
        let salvo;
        if (idEdicao) {
            salvo = await requisitarApi(`/api/produtos/${encodeURIComponent(idEdicao)}`, {
                method: "PUT",
                body: JSON.stringify({ nome, categoria, preco, estoque, ativo })
            });
        } else {
            salvo = await requisitarApi("/api/produtos", {
                method: "POST",
                body: JSON.stringify({ nome, categoria, preco, estoque })
            });
        }

        const acaoTexto = idEdicao ? "atualizado" : "cadastrado";
        prepararFormularioNovoProduto();
        definirEstadoContainer(
            "estado-form-produto",
            "success",
            `Produto "${salvo.nome}" ${acaoTexto} com sucesso (Estoque: ${salvo.estoque}).`,
            {
                rotulo: "Ver ficha do produto",
                callback: () => abrirDetalheProduto(salvo.id)
            }
        );

        await Promise.all([carregarProdutos(), carregarOpcoesProdutosSelects()]);
    } catch (erro) {
        definirEstadoContainer("estado-form-produto", "error", erro.message, {
            rotulo: "Tentar novamente",
            callback: salvarProdutoFormulario
        });
    }
}

function selecionarProdutoParaPedido(produtoId) {
    ativarVisao("visao-formulario");
    const select = document.getElementById("pedido-produto-id");
    select.value = produtoId;
    atualizarPreviaPedido();
    document.getElementById("pedido-nome-cliente").focus();
}

function atualizarPreviaPedido() {
    const produtoId = document.getElementById("pedido-produto-id").value;
    const quantidade = Math.max(0, Number(document.getElementById("pedido-quantidade").value) || 0);
    const cupom = document.getElementById("pedido-cupom").value.trim();

    const produto = estadoApp.catalogoCache.find((p) => p.id === produtoId);
    if (!produto || quantidade <= 0) {
        document.getElementById("previa-subtotal").textContent = "R$ 0,00";
        document.getElementById("previa-desconto").textContent = "R$ 0,00";
        document.getElementById("previa-frete").textContent = "R$ 0,00";
        document.getElementById("previa-total").textContent = "R$ 0,00";
        return;
    }

    const subtotal = arredondar2(produto.preco * quantidade);
    const aplicaDesc10 = cupom.toUpperCase() === "DESC10" || quantidade >= 10;
    const desconto = aplicaDesc10 ? arredondar2(subtotal * 0.1) : 0;
    const valorComDesconto = arredondar2(subtotal - desconto);
    const frete = valorComDesconto >= 100 ? 0 : 15;
    const total = arredondar2(valorComDesconto + frete);

    document.getElementById("previa-subtotal").textContent = formatarMoeda(subtotal);
    document.getElementById("previa-desconto").textContent = formatarMoeda(desconto);
    document.getElementById("previa-frete").textContent = frete === 0 ? "R$ 0,00 (Gratis)" : formatarMoeda(frete);
    document.getElementById("previa-total").textContent = formatarMoeda(total);
}

async function emitirPedidoFormulario() {
    const produtoId = document.getElementById("pedido-produto-id").value.trim();
    const nomeCliente = document.getElementById("pedido-nome-cliente").value.trim();
    const quantidade = Number(document.getElementById("pedido-quantidade").value);
    const cupomBruto = document.getElementById("pedido-cupom").value.trim();
    const cupom = cupomBruto ? cupomBruto : null;

    definirEstadoContainer("estado-form-pedido", "loading", "Carregando dados...");

    try {
        const criado = await requisitarApi("/api/pedidos", {
            method: "POST",
            body: JSON.stringify({
                produtoId,
                nomeCliente,
                quantidade,
                cupom
            })
        });

        document.getElementById("form-pedido").reset();
        document.getElementById("pedido-quantidade").value = "1";

        await Promise.all([
            carregarProdutos(),
            carregarOpcoesProdutosSelects(),
            carregarPedidos()
        ]);

        definirEstadoContainer(
            "estado-form-pedido",
            "success",
            `Pedido confirmado para ${criado.nomeCliente}: ${criado.quantidade}x ${criado.nomeProduto} | Total: ${formatarMoeda(criado.valorTotal)} (Desconto: ${formatarMoeda(criado.desconto)}, Frete: ${formatarMoeda(criado.frete)}).`,
            {
                rotulo: "Ver pedidos deste produto",
                callback: () => abrirDetalheProduto(criado.produtoId)
            }
        );
    } catch (erro) {
        definirEstadoContainer("estado-form-pedido", "error", erro.message, {
            rotulo: "Tentar novamente",
            callback: emitirPedidoFormulario
        });
    }
}

// Utilitarios de formatacao e escape contra XSS
function arredondar2(valor) {
    return Math.round((valor + Number.EPSILON) * 100) / 100;
}

function formatarMoeda(valor) {
    const numero = Number(valor) || 0;
    return numero.toLocaleString("pt-BR", {
        style: "currency",
        currency: "BRL"
    });
}

function formatarData(isoString) {
    if (!isoString) {
        return "-";
    }
    const data = new Date(isoString);
    if (Number.isNaN(data.getTime())) {
        return "-";
    }
    return data.toLocaleString("pt-BR", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit"
    });
}

function escaparHtml(texto) {
    if (texto === null || texto === undefined) {
        return "";
    }
    return String(texto)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#39;");
}
