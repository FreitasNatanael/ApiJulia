namespace ApiJulia.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int CustomerId { get; set; }

        public decimal Total { get; set; }

        public required List<OrderItem> Items { get; set; }
    }
}
