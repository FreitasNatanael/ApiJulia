using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiJulia.Models; // Ajuste isso se a sua pasta de Models tiver outro nome

namespace ApiJulia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            // O .Include traz os itens do pedido junto com a resposta
            return await _context.Orders
                .Include(o => o.Items)
                .ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder([FromBody] CreateOrderDto dto)
        {
            // REGRA 1: Não permitir pedidos sem itens
            if (dto.Items == null || !dto.Items.Any())
            {
                return BadRequest(new { error = "O pedido deve conter pelo menos um item" });
            }

            // Criando a "casca" do pedido
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                Date = DateTime.Now,
                Total = 0,
                Items = new List<OrderItem>()
            };

            // Avaliando cada item da cestinha
            foreach (var itemDto in dto.Items)
            {
                // Busca o produto no banco de dados para pegar preço e estoque reais
                var product = await _context.Products.FindAsync(itemDto.ProductId);

                if (product == null)
                {
                    return NotFound(new { error = $"Produto com ID {itemDto.ProductId} não encontrado" });
                }

                // REGRA 2: Não permitir criar pedido com produtos sem estoque suficiente
                if (product.Stock < itemDto.Quantity)
                {
                    return BadRequest(new { error = "Produto sem estoque suficiente" });
                }

                // REGRA 3: O preço do produto deve ser armazenado no momento da compra
                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price // Pegando do banco, evitando fraudes
                };

                // REGRA 4: O total do pedido deve ser calculado corretamente
                order.Total += (orderItem.Quantity * orderItem.UnitPrice);

                // REGRA 5: Ao criar um pedido, o estoque do produto deve ser atualizado
                product.Stock -= orderItem.Quantity;

                // Adiciona o item montado dentro do pedido
                order.Items.Add(orderItem);
            }

            // Salva no banco de dados: O Pedido, os Itens e a atualização do Estoque dos produtos
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Retorna o status 201 e o pedido pronto
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
    }

    // --- CLASSES DTO ---
    // Usadas para receber apenas os dados necessários para criar o pedido

    public class CreateOrderDto
    {
        public int CustomerId { get; set; }
        public required List<CreateOrderItemDto> Items { get; set; }
    }

    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}