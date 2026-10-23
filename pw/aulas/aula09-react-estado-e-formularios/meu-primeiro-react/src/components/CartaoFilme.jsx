import { useState } from "react";

function CartaoFilme({ filme }) {
    const [curtidas, setCurtidas] = useState(0);

    return (
        <article className="cartao">
            <h2>{filme.titulo}</h2>
            <p>{filme.ano} · {filme.genero} · {filme.duracao} min</p>
            <button onClick={() => setCurtidas(curtidas + 1)}>
                Curtir ({curtidas})
            </button>
        </article>
    );
}

export default CartaoFilme;