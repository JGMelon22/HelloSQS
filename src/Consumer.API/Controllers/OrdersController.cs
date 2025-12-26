using Amazon.SQS.Model;
using AmazonSQS.Core.Domains.DTOs.Requests;
using AmazonSQS.Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Consumer.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderCreatedEventRequest orderCreatedEventRequest)
    {
        SendMessageResponse response = await orderService.CreateOrderAsync(orderCreatedEventRequest);
        return Ok(response);
    }
}
