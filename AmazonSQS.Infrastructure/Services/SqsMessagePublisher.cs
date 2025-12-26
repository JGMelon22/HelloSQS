using Amazon.SQS;
using Amazon.SQS.Model;
using AmazonSQS.Infrastructure.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AmazonSQS.Infrastructure.Services;

public class SqsMessagePublisher(IAmazonSQS amazonSQS, ILogger<SqsMessagePublisher> logger) : ISqsMessagePublisher
{
    public async Task PublishAsync<T>(T message, string queueUrl, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            string messageBody = JsonSerializer.Serialize(message);

            SendMessageRequest sendMessage = new()
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody
            };

            logger.LogInformation("Publishing message to queue {queueUrl}. Message type: {messageType}", queueUrl, typeof(T).Name);

            SendMessageResponse response = await amazonSQS.SendMessageAsync(sendMessage);

            logger.LogInformation(
               "Message published successfully. MessageId: {messageId}, QueueUrl: {queueUrl}",
               response.MessageId,
               queueUrl
           );
        }
        catch (AmazonSQSException ex)
        {
            logger.LogError(ex,
                "AWS SQS error while publishing message to queue {queueUrl}. Error Code: {errorCode}",
                queueUrl,
                ex.ErrorCode
            );
            throw;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex,
                "Failed to serialize message of type {messageType}",
                typeof(T).Name
            );
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while publishing message to queue {queueUrl}",
                queueUrl
            );
            throw;
        }
    }
}
