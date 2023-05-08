using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.Api.Contexts;

namespace Stock.Api.Consumers;

public  class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<OrderCreatedEventConsumer> _logger;
    private readonly ISendEndpointProvider _sendEndpoint;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderCreatedEventConsumer(AppDbContext dbContext, ILogger<OrderCreatedEventConsumer> logger, ISendEndpointProvider sendEndpoint, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _logger = logger;
        _sendEndpoint = sendEndpoint;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var stockResult = new List<bool>();
        foreach (var item in context.Message.OrderItems)
        {
            stockResult.Add(await _dbContext.Stocks.AnyAsync(x=>x.ProductId==item.ProductId && x.Count>item.Count));
        }

        if (stockResult.All(x=>x.Equals(true)))
        {
            foreach (var item in context.Message.OrderItems)
            {
                var stock = await _dbContext.Stocks.FirstOrDefaultAsync(x=>x.ProductId.Equals(item.ProductId));
                if (stock is not null)
                {
                    stock.Count -= item.Count;
                }
                
                await _dbContext.SaveChangesAsync();
            }
            _logger.LogInformation($"Stock was reserved Buyer Id : {context.Message.BuyerId}");

            var sendEndpoint = await _sendEndpoint.GetSendEndpoint(new Uri($"queue:{RabbitMqSettings.StockReservedEventQueueName}"));

            StockReservedEvent stockReservedEvent = new StockReservedEvent()
            {
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId,
                PaymentMessage = context.Message.Payment,
                OrderItems = context.Message.OrderItems
            };

            await sendEndpoint.Send(stockReservedEvent);
            
        }
        else
        {
            await _publishEndpoint.Publish(new StockNotReservedEvent()
            {
                BuyerId = context.Message.BuyerId,
                Message = "Not enough Stock",
                OrderId = context.Message.OrderId
            });
        }
    }
}