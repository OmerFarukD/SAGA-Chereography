using MassTransit;
using Order.Api.Contexts;
using Order.Api.Models;
using Shared;

namespace Order.Api.Consumers;

public class PaymentSuccessEventConsumer : IConsumer<PaymentSuccessEvent>
{

    private readonly AppDbContext _appDbContext;
    private readonly ILogger<PaymentSuccessEventConsumer>_logger;

    public PaymentSuccessEventConsumer(AppDbContext appDbContext, ILogger<PaymentSuccessEventConsumer> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<PaymentSuccessEvent> context)
    {
        var order = await _appDbContext.Orders.FindAsync(context.Message.OrderId);
        if (order is not null)
        {
            order.OrderStatus = OrderStatus.SUCCESS;
            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation($"Order Id : {context.Message.OrderId} -> Status : {order.OrderStatus}");
        }
        else
        {
            _logger.LogInformation($"Order Id : {context.Message.OrderId} is not found");
        }
        
        
    }
}