using ApiJulia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiJulia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {

        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<CustomerController>
        [HttpGet]
      
        public async Task<ActionResult<IEnumerable<Customer>>> Get()
        {
            // O "_context" é a sua ligação com o banco. 
            // "Customers" é a tabela de clientes.
            // "ToListAsync()" diz: "Pegue tudo o que encontrar e transforme em uma lista".
            return await _context.Customers.ToListAsync();
        }

        // GET api/Customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> Get(int id)
        {
            // Vai no banco e procura o cliente que tem esse ID específico
            var customer = await _context.Customers.FindAsync(id);

            // Se não encontrar ninguém com esse ID, avisa que não existe (Erro 404)
            if (customer == null)
            {
                return NotFound();
            }

            // Se encontrou, devolve os dados do cliente
            return customer;
        }

        // POST api/Customer
        [HttpPost]
        public async Task<ActionResult<Customer>> Post([FromBody] Customer customer)
        {
            // 1. Prepara o cliente para ser salvo
            _context.Customers.Add(customer);

            // 2. Salva de verdade no banco de dados (é aqui que o SQL trabalha)
            await _context.SaveChangesAsync();

            // 3. Devolve a resposta de sucesso e avisa onde o cliente novo pode ser encontrado
            return CreatedAtAction(nameof(Get), new { id = customer.Id }, customer);
        }


        // PUT api/Customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer customer)
        {
            // 1. Verifica se o ID da URL é o mesmo ID do corpo (JSON)
            if (id != customer.Id)
            {
                return BadRequest(); // Retorna erro 400
            }

            // 2. Avisa o Entity Framework que os dados desse cliente foram modificados
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                // 3. Tenta salvar as alterações no banco de dados
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 4. Se der erro ao salvar, verifica se o cliente realmente existe
                if (!CustomerExists(id))
                {
                    return NotFound(); // Retorna erro 404 se o cliente não existir
                }
                else
                {
                    throw; // Se for outro erro grave, a aplicação avisa
                }
            }

            // 5. Retorna sucesso, mas sem precisar devolver nada na tela
            return NoContent(); // Retorna 204
        }

        // Método auxiliar (ajudante) para verificar se o cliente existe
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        // DELETE api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // 1. Vai no banco de dados e procura o cliente com esse ID
            var customer = await _context.Customers.FindAsync(id);

            // 2. Se o cliente não existir, retorna erro 404 (Não Encontrado)
            if (customer == null)
            {
                return NotFound();
            }

            // 3. Avisa o Entity Framework para remover esse cliente da tabela
            _context.Customers.Remove(customer);

            // 4. Salva a alteração (apaga de verdade no SQL)
            await _context.SaveChangesAsync();

            // 5. Retorna sucesso (204) avisando que deu tudo certo e não há mais nada a mostrar
            return NoContent();
        }
    }
}
