using MassTransit;
using Order.Api.Contexts;
using Order.Api.Models;
using Shared;

namespace Order.Api.Consumers;

public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly ILogger<PaymentFailedEventConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly AppDbContext _appDbContext;

    public PaymentFailedEventConsumer(ILogger<PaymentFailedEventConsumer> logger, IPublishEndpoint publishEndpoint, AppDbContext appDbContext)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _appDbContext = appDbContext;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var order = await _appDbContext.Orders.FindAsync(context.Message.OrderId);
        if (order is not null)
        {
            order.OrderStatus = OrderStatus.FAIL;
            order.FailMessage = context.Message.Message;
            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation($"Order Id : {context.Message.OrderId} -> Status : {order.OrderStatus}");
        }
        else
        {
            _logger.LogInformation($"Order Id : {context.Message.OrderId} is not found");
        }
    }
}