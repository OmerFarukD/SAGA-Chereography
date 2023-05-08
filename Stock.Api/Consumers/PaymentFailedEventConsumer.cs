using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.Api.Contexts;

namespace Stock.Api.Consumers;

public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly AppDbContext _appDbContext;
    private readonly ILogger<PaymentFailedEventConsumer> _logger;

    public PaymentFailedEventConsumer(AppDbContext appDbContext, ILogger<PaymentFailedEventConsumer> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        foreach (var messageOrderItemMessage in context.Message.OrderItemMessages)
        {
            var stock = await _appDbContext.Stocks.FirstOrDefaultAsync(x=>x.ProductId.Equals(messageOrderItemMessage.ProductId));

            if (stock is not null)
            {
                stock.Count += messageOrderItemMessage.Count;
                await _appDbContext.SaveChangesAsync();
            }
        }
        _logger.LogInformation($"Stock was released for OrderId : {context.Message.OrderId}");
    }
}