using MassTransit;
using Order.Api.Contexts;
using Order.Api.Models;
using Shared;

namespace Order.Api.Consumers;

public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
{
    private readonly ILogger<StockNotReservedEventConsumer> _logger;
 
    private readonly AppDbContext  _appDbContext;

    public StockNotReservedEventConsumer(ILogger<StockNotReservedEventConsumer> logger,  AppDbContext appDbContext)
    {
        _logger = logger;
        _appDbContext = appDbContext;
    }

    public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
    {
        var order = await _appDbContext.Orders.FindAsync(context.Message.OrderId);
        if (order is not null)
        {
            order.OrderStatus = OrderStatus.FAIL;
            order.FailMessage = context.Message.Message;
            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation($"Order Id : {context.Message.OrderId} -> Status : {order.OrderStatus} because out of stock");
        }
        else
        {
            _logger.LogInformation($"Order Id : {context.Message.OrderId} is not found");
        }
    }
}