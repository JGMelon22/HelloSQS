    using Amazon.SQS.Model;
    using AmazonSQS.Core.Domains.DTOs.Requests;
    using AmazonSQS.Infrastructure.Interfaces.Services;
    using Microsoft.AspNetCore.Mvc;

    namespace Producer.API.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] OrderCreatedEventRequest orderCreatedEventRequest)
        {
            await orderService.CreateOrderAsync(orderCreatedEventRequest);
            return Ok();
        }

        [HttpPost("messages")]
        public async Task<IActionResult> CreateOrderBatchesAsync(
            [FromBody] ICollection<OrderCreatedEventRequest> orderCreatedEventRequest)
        {
            await orderService.CreateOrderBatchAsync(orderCreatedEventRequest);
            return Ok();
        }
    }