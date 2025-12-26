using Amazon.SQS;
using Amazon.SQS.Model;
using AmazonSQS.Infrastructure.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Consumer.Consumer.BackgroundServices;

public class OrderCreatedEventConsumer(
    ILogger<OrderCreatedEventConsumer> logger,
    IAmazonSQS sqsService, IOptions<SqsOptions> sqsOptions
    ) : BackgroundService
{
    private readonly SqsOptions _sqsOptions = sqsOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string queueUrl = _sqsOptions.OrderCreatedQueueUrl;
        logger.LogInformation("Polling Queue {queueName}", queueUrl);

        ReceiveMessageRequest receivedRequest = new()
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = 10,
            WaitTimeSeconds = 20
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await sqsService.ReceiveMessageAsync(receivedRequest);

                if (response.Messages.Count > 0)
                {
                    foreach (Message message in response.Messages)
                    {
                        logger.LogInformation("Received Message from Queue {queueName} with body as : \n {body}", queueUrl, message.Body);
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        await sqsService.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Queue polling cancelled");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error polling queue {QueueUrl}", queueUrl);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}