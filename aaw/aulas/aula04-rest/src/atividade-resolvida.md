# Atividade resolvida — PetShopApi

1. Eu chamei POST /api/v1/getPets. O problema é que a URI tem verbo e o método está errado para leitura. O certo é usar GET /api/v1/pets?page=&size= com paginação.

2. Eu chamei GET /api/v1/deletarPet?id=7. O problema é que um GET está apagando dados. O certo é DELETE /api/v1/pets/{id}, com 204 quando apagar e 404 se não existir.

3. Eu chamei GET /api/v1/pet/{id}. O problema é que o recurso aparece no singular. O certo é manter só GET /api/v1/pets/{id}.

4. Eu chamei GET /api/v1/banhosTosa e GET /api/v1/tutores_vip. O problema é que a nomenclatura muda sem padrão. O certo é usar GET /api/v1/banhos-e-tosas e GET /api/v1/tutores?vip=true.

5. Eu chamei POST /api/v1/pets. O problema é que a criação volta com 200 OK e sem Location. O certo é devolver 201 Created com o header Location.

6. Eu chamei GET /api/v1/pets/{id} com um id que não existe. O problema é que o erro volta como sucesso. O certo é responder 404 Not Found com application/problem+json.

7. Eu chamei GET /api/pets. O problema é que a rota não tem versão e ainda muda o contrato. O certo é manter GET /api/v1/pets e criar GET /api/v2/pets quando mudar o formato.

8. Eu chamei GET /api/v1/petshops/1/clientes/5/pets/9/consultas/12/exames/6. O problema é que a URL tem aninhamento demais. O certo é usar GET /api/v1/exames/{id} ou, no máximo, GET /api/v1/consultas/{consultaId}/exames.

9. Eu chamei GET /api/v1/consultas. O problema é que ele devolve tudo sem paginação. O certo é usar GET /api/v1/consultas?page=&size=&petId=&veterinario=&sort=.

10. Eu chamei PUT /api/v1/pets/{id}/vacinas. O problema é que repetir a chamada acumula vacinas, então não é idempotente. O certo é usar POST para registrar nova dose ou PUT para substituir a carteira inteira.

11. Eu chamei POST /api/v1/sessao e depois GET /api/v1/meus-pets. O problema é que o servidor guarda sessão em memória. O certo é mandar o contexto em cada requisição, usando token ou GET /api/v1/tutores/{tutorId}/pets.

12. Eu chamei GET /api/v1/tabela-de-precos. O problema é que a resposta bloqueia cache mesmo sendo um dado quase fixo. O certo é usar ETag e Cache-Control: public, max-age=3600, com suporte a 304 Not Modified.
