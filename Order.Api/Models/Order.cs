namespace Order.Api.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? BuyerId { get; set; }
    public Address Address { get; set; } = new Address();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public OrderStatus OrderStatus { get; set; }
    public string? FailMessage { get; set; }
}