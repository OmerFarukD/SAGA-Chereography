using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Contexts;
using Order.Api.Dtos;
using Order.Api.Models;
using Shared;

namespace Order.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrdersController(AppDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] OrderCreateDto orderCreateDto)
    {
        var newOrder = new Models.Order()
        {
            BuyerId = orderCreateDto.BuyerId,
            OrderStatus = OrderStatus.SUSPEND,
            Address = new Address()
            {
                Line = orderCreateDto.Address.Line,
                District = orderCreateDto.Address.District,
                Province= orderCreateDto.Address.Province
            },
            CreatedTime = DateTime.Now
        };
        
        orderCreateDto.OrderItems.ForEach(item =>
        {
            newOrder.OrderItems.Add(new OrderItem(){
                Count = item.Count,
                Price = item.Price,
                ProductId = item.ProductId
            });
        });
        await _dbContext.AddAsync(newOrder);
        await _dbContext.SaveChangesAsync();


        var orderCreatedEvent = new OrderCreatedEvent()
        {
            BuyerId = orderCreateDto.BuyerId,
            OrderId = newOrder.Id,
            Payment = new PaymentMessage()
            {
                CardName = orderCreateDto.Payment.CardName,
                Cvv = orderCreateDto.Payment.Cvv,
                Expiration = orderCreateDto.Payment.Expiration,
                CardNumber = orderCreateDto.Payment.CardNumber,
                TotalPrice = orderCreateDto.OrderItems.Sum(x => x.Price*x.Count)
            }
        };
        
        orderCreateDto.OrderItems.ForEach(item =>
        {
            orderCreatedEvent.OrderItems.Add(new OrderItemMessage(){Count = item.Count, ProductId = item.ProductId});
        });

        await _publishEndpoint.Publish(orderCreatedEvent);
        return Ok();
    }
    
}