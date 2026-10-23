import { useState } from "react";
import filmes from "./dados/filmes";
import CartaoFilme from "./components/CartaoFilme";

function App() {
  const [busca, setBusca] = useState("");

  const filtrados = filmes.filter((f) =>
    f.titulo.toLowerCase().includes(busca.toLowerCase())
  );

  return (
    <>
      <h1 className="titulo">Catálogo de filmes</h1>

      <input
        placeholder="Buscar pelo título..."
        value={busca}
        onChange={(evento) => setBusca(evento.target.value)}
      />
      <p>{filtrados.length} de {filmes.length} filmes</p>

      {filtrados.length === 0 && <p>Nenhum filme encontrado.</p>}

      <div className="lista">
        {filtrados.map((filme) => (
          <CartaoFilme key={filme.id} filme={filme} />
        ))}
      </div>
    </>
  );
}

export default App;
