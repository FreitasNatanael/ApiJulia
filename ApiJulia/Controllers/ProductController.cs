using ApiJulia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiJulia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<ProductController>
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            // O "_context" é a sua ligação com o banco. 
            // "Products" é a tabela de clientes.
            // "ToListAsync()" diz: "Pegue tudo o que encontrar e transforme em uma lista".
            return await _context.Products.ToListAsync();
        }

        // GET api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            // Vai no banco e procura o cliente que tem esse ID específico
            var Product = await _context.Products.FindAsync(id);

            // Se não encontrar ninguém com esse ID, avisa que não existe (Erro 404)
            if (Product == null)
            {
                return NotFound();
            }

            // Se encontrou, devolve os dados do cliente
            return Product;
        }

        // POST api/Product
        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody] Product Product)
        {
            // 1. Prepara o cliente para ser salvo
            _context.Products.Add(Product);

            // 2. Salva de verdade no banco de dados (é aqui que o SQL trabalha)
            await _context.SaveChangesAsync();

            // 3. Devolve a resposta de sucesso e avisa onde o cliente novo pode ser encontrado
            return CreatedAtAction(nameof(Get), new { id = Product.Id }, Product);
        }


        // PUT api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Product Product)
        {
            // 1. Verifica se o ID da URL é o mesmo ID do corpo (JSON)
            if (id != Product.Id)
            {
                return BadRequest(); // Retorna erro 400
            }

            // 2. Avisa o Entity Framework que os dados desse cliente foram modificados
            _context.Entry(Product).State = EntityState.Modified;

            try
            {
                // 3. Tenta salvar as alterações no banco de dados
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 4. Se der erro ao salvar, verifica se o cliente realmente existe
                if (!ProductExists(id))
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
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // DELETE api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // 1. Vai no banco de dados e procura o cliente com esse ID
            var Product = await _context.Products.FindAsync(id);

            // 2. Se o cliente não existir, retorna erro 404 (Não Encontrado)
            if (Product == null)
            {
                return NotFound();
            }

            // 3. Avisa o Entity Framework para remover esse cliente da tabela
            _context.Products.Remove(Product);

            // 4. Salva a alteração (apaga de verdade no SQL)
            await _context.SaveChangesAsync();

            // 5. Retorna sucesso (204) avisando que deu tudo certo e não há mais nada a mostrar
            return NoContent();
        }
    }
}
