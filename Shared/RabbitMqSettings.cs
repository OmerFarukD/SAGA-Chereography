namespace Shared;

public class RabbitMqSettings
{
    public const string StockReservedEventQueueName = "stock-reserved-queue-name";
    public const string StockOrderCreatedEventQueueName = "stock-order-created-queue";
    public const string OrderPaymentSuccessEventQueueName = "order-payment-success-queue";
    public const string OrderPaymentFailedEventQueueName = "order-payment-failed-queue";
    public const string StockNotReservedEventQueueName = "stock-not-reserved-queue-name";
    public const string StockPaymentFailedEventQueueName = "stock-payment-faled-event-queue";
}