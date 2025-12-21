using Amazon.SQS;
using Amazon.SQS.Model;
using Messages;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Consumer.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<OrdersController> _logger;
    private const string OrderCreatedEventQueueName = "order-created";

    public OrdersController(IAmazonSQS sqsClient, ILogger<OrdersController> logger)
    {
        _sqsClient = sqsClient;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync()
    {
        OrderCreatedEvent @event = new(Guid.NewGuid(), Guid.NewGuid());
        string queueUrl = await GetQueueUrl(OrderCreatedEventQueueName);
        SendMessageRequest sendMessageRequest = new()
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(@event)
        };

        _logger.LogInformation("Publishing message to Queue {queueName} with body : \n {request}", OrderCreatedEventQueueName, sendMessageRequest.MessageBody);
        SendMessageResponse result = await _sqsClient.SendMessageAsync(sendMessageRequest);
        return Ok(result);
    }

    private async Task<string> GetQueueUrl(string queueName)
    {
        try
        {
            GetQueueUrlResponse response = await _sqsClient.GetQueueUrlAsync(queueName);
            return response.QueueUrl;
        }
        catch (QueueDoesNotExistException ex)
        {
            _logger.LogWarning(ex, "Queue {queueName} doesn't exist. Creating...", queueName);
            CreateQueueResponse response = await _sqsClient.CreateQueueAsync(queueName);
            return response.QueueUrl;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get or create queue {queueName}", queueName);
            throw;
        }
    }
}
