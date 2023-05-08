namespace Order.Api.Dtos;

    public sealed class OrderCreateDto
    {
        public string? BuyerId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public PaymentDto Payment { get; set; }
        public AddressDto Address { get; set; }
    }