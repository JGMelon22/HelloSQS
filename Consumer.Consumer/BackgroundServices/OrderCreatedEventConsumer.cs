using Amazon.SQS;
using Amazon.SQS.Model;

namespace Consumer.Consumer.BackgroundServices;

public class OrderCreatedEventConsumer : BackgroundService
{
    private readonly ILogger<OrderCreatedEventConsumer> _logger;
    private readonly IAmazonSQS _sqsService;
    private const string OrderCreatedEventQueueName = "order-created";

    public OrderCreatedEventConsumer(ILogger<OrderCreatedEventConsumer> logger, IAmazonSQS sqsService)
    {
        _logger = logger;
        _sqsService = sqsService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Polling Queue {queueName}", OrderCreatedEventQueueName);
        string queueUrl = await GetQueueUrl(OrderCreatedEventQueueName);
        ReceiveMessageRequest receivedRequest = new()
        {
            QueueUrl = queueUrl
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            ReceiveMessageResponse response = await _sqsService.ReceiveMessageAsync(receivedRequest);

            while (response.Messages.Count > 0)
            {
                foreach (Message message in response.Messages)
                {
                    _logger.LogInformation("Received Message from Queue {queueName} with body as : \n {body}", OrderCreatedEventQueueName, message.Body);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    await _sqsService.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                }
            }
        }
    }

    private async Task<string> GetQueueUrl(string queueName)
    {
        try
        {
            GetQueueUrlResponse response = await _sqsService.GetQueueUrlAsync(queueName);
            return response.QueueUrl;
        }
        catch (QueueDoesNotExistException ex)
        {
            _logger.LogWarning(ex, "Queue {queueName} doesn't exist. Creating...", queueName);
            CreateQueueResponse response = await _sqsService.CreateQueueAsync(queueName);
            return response.QueueUrl;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get or create queue {queueName}", queueName);
            throw;
        }
    }
}
