/**
 * ============================================================================
 * api-falsa.js — MATERIAL DE APOIO DA AULA 17 (React III)
 * Programação Web — Newton Paiva
 * ============================================================================
 *
 * PARA QUE SERVE
 * Uma API de mentira, com a MESMA assinatura de uma API de verdade: funções
 * assíncronas que devolvem Promise, demoram para responder e podem falhar.
 * Serve para você exercitar os quatro estados da tela sem depender de rede,
 * de chave de acesso ou de um serviço público estar no ar:
 *
 *     carregando  → controlado pelo parâmetro atraso
 *     erro        → controlado pelo parâmetro falha
 *     vazio       → controlado pelo parâmetro vazio
 *     sucesso     → o padrão
 *
 * Sem isso, testar o estado de erro exige desligar a internet no meio da aula,
 * e testar o estado vazio exige apagar dados. Aqui é um parâmetro.
 *
 * ---------------------------------------------------------------------------
 * COMO USAR NO SEU PROJETO REACT
 * ---------------------------------------------------------------------------
 *   1. copie este arquivo para src/ do seu projeto;
 *   2. importe e use dentro de um useEffect:
 *
 *      import { buscarItens } from './api-falsa.js';
 *
 *      useEffect(() => {
 *        let cancelado = false;                    // ver a Rodada 4
 *        setCarregando(true);
 *        setErro('');
 *
 *        buscarItens({ atraso: 1200 })
 *          .then((dados) => { if (!cancelado) setItens(dados); })
 *          .catch((e)   => { if (!cancelado) setErro(e.message); })
 *          .finally(()  => { if (!cancelado) setCarregando(false); });
 *
 *        return () => { cancelado = true; };       // limpeza
 *      }, []);
 *
 *   3. para forçar cada estado, mude só os parâmetros:
 *
 *      buscarItens({ atraso: 3000 })            // carregando bem visível
 *      buscarItens({ falha: true })             // erro
 *      buscarItens({ vazio: true })             // sucesso com lista vazia
 *      buscarItens()                            // sucesso, atraso padrão
 *
 * ---------------------------------------------------------------------------
 * A TROCA PELA API DE VERDADE (Passo 5 da prática)
 * ---------------------------------------------------------------------------
 * Como o resto do projeto só conhece os NOMES buscarItens e buscarItemPorId,
 * trocar a fonte de dados é mexer em um arquivo só: escreva um api.js com as
 * mesmas duas funções usando fetch, e mude apenas a linha do import.
 * Nenhum componente precisa saber de onde vêm os dados — é o mesmo raciocínio
 * de fronteira entre módulos da aula 14.
 *
 * ---------------------------------------------------------------------------
 * OBSERVAÇÕES PARA O PROFESSOR
 * ---------------------------------------------------------------------------
 * · buscarItemPorId tem atraso VARIÁVEL de propósito (ids menores respondem
 *   mais devagar). É o que faz a condição de corrida da Rodada 4 acontecer de
 *   forma reproduzível: clicar no item 1 e logo depois no item 5 faz a
 *   resposta do 1 chegar DEPOIS e sobrescrever a do 5.
 * · Nada aqui usa rede. Funciona offline, no laboratório e em casa.
 * · Os dados são os mesmos das aulas 11 e 13, de propósito: a turma reconhece.
 * ============================================================================
 */

const ITENS = [
  { id: 1, nome: "Mouse óptico", categoria: "perifericos", preco: 89.90, quantidade: 42, disponivel: true },
  { id: 2, nome: "Teclado mecânico", categoria: "perifericos", preco: 349.00, quantidade: 18, disponivel: true },
  { id: 3, nome: "Monitor 24 polegadas", categoria: "monitores", preco: 899.00, quantidade: 7, disponivel: true },
  { id: 4, nome: "Headset com microfone", categoria: "perifericos", preco: 229.90, quantidade: 25, disponivel: true },
  { id: 5, nome: "Cabo HDMI 2 m", categoria: "cabos", preco: 39.90, quantidade: 90, disponivel: true },
  { id: 6, nome: "SSD 480 GB", categoria: "armazenamento", preco: 279.00, quantidade: 12, disponivel: true },
  { id: 7, nome: "Webcam Full HD", categoria: "perifericos", preco: 199.00, quantidade: 0, disponivel: false },
  { id: 8, nome: "Hub USB 4 portas", categoria: "cabos", preco: 79.90, quantidade: 33, disponivel: true },
];

/** Detalhe extra, devolvido só na busca por id — como fazem as APIs de verdade. */
const DETALHES = {
  1: "Sensor óptico de 1600 dpi, 3 botões, cabo de 1,8 m.",
  2: "Switches azuis, ABNT2, iluminação branca, apoio removível.",
  3: "IPS, 1920×1080, 75 Hz, entradas HDMI e VGA.",
  4: "Microfone com haste flexível e controle de volume no fio.",
  5: "Compatível com 4K a 60 Hz, conectores banhados a ouro.",
  6: "Leitura de até 550 MB/s, formato 2,5 polegadas, SATA III.",
  7: "1920×1080 a 30 fps, microfone embutido, foco fixo.",
  8: "USB 2.0, alimentação pelo próprio cabo, 4 portas.",
};

function esperar(ms) {
  return new Promise((resolver) => setTimeout(resolver, ms));
}

/**
 * Lista os itens.
 * @param {{atraso?: number, falha?: boolean, vazio?: boolean}} [opcoes]
 * @returns {Promise<Array>} os itens (ou uma Promise rejeitada, se falha)
 */
export async function buscarItens(opcoes = {}) {
  const { atraso = 900, falha = false, vazio = false } = opcoes;

  await esperar(atraso);

  if (falha) {
    // Mensagem no formato de um erro de HTTP, para você exercitar o texto que
    // o USUÁRIO vai ler — que não deve ser este.
    throw new Error("HTTP 503 — servico temporariamente indisponivel");
  }

  if (vazio) return [];

  // Cópia rasa, para o componente não conseguir alterar a "base de dados"
  // por acidente — é o que uma API de verdade também garantiria.
  return ITENS.map((item) => ({ ...item }));
}

/**
 * Busca um item pelo id, com detalhe extra.
 *
 * ATENÇÃO: o atraso é VARIÁVEL de propósito — ids menores demoram mais.
 * É isso que torna a condição de corrida reproduzível: peça o id 1, logo em
 * seguida o id 5, e observe qual resposta chega por último.
 *
 * @param {number} id
 * @param {{falha?: boolean}} [opcoes]
 * @returns {Promise<Object>}
 */
export async function buscarItemPorId(id, opcoes = {}) {
  const { falha = false } = opcoes;

  // id 1 → 2000 ms ... id 8 → 300 ms
  const atraso = Math.max(300, 2200 - Number(id) * 250);
  await esperar(atraso);

  if (falha) throw new Error("HTTP 500 — erro ao buscar o item " + id);

  const item = ITENS.find((i) => i.id === Number(id));
  if (!item) throw new Error("HTTP 404 — item " + id + " nao encontrado");

  return { ...item, detalhe: DETALHES[item.id] || "sem descricao cadastrada" };
}

/** Só para inspeção no console durante o laboratório. Não use nos componentes. */
export const TOTAL_DE_ITENS = ITENS.length;
