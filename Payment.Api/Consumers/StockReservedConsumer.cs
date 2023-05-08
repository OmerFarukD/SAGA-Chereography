using MassTransit;
using Shared;

namespace Payment.Api.Consumers;

public class StockReservedConsumer : IConsumer<StockReservedEvent>
{
    private readonly ILogger<StockReservedConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public StockReservedConsumer(ILogger<StockReservedConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        var balance = 3000m;
        if (balance>context.Message.PaymentMessage.TotalPrice)
        {
            _logger.LogInformation($"{context.Message.PaymentMessage.TotalPrice} TL çekildi User Id: {context.Message.BuyerId}");

           await _publishEndpoint.Publish(new PaymentSuccessEvent()
                { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId });
        }
        else
        {
            _logger.LogInformation($"Bakiye yetersiz.");
            await _publishEndpoint.Publish(new PaymentFailedEvent { BuyerId = context.Message.BuyerId,OrderItemMessages =context.Message.OrderItems ,OrderId = context.Message.OrderId, Message = "not enough balance"});
        }
    }
}