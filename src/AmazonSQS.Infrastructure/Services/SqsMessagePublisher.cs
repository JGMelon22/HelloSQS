using System.Collections.ObjectModel;
using Amazon.SQS;
using Amazon.SQS.Model;
using AmazonSQS.Infrastructure.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AmazonSQS.Infrastructure.Services;

public class SqsMessagePublisher(IAmazonSQS amazonSqs, ILogger<SqsMessagePublisher> logger) : ISqsMessagePublisher
{
    public async Task<SendMessageResponse?> PublishAsync<T>(T message, string queueUrl,
        CancellationToken cancellationToken = default) where T : class
    {
        string messageBody;

        try
        {
            messageBody = JsonSerializer.Serialize(message);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex,
                "Failed to serialize message of type {MessageType}",
                typeof(T).Name
            );
            return null;
        }

        SendMessageRequest sendMessage = new()
        {
            QueueUrl = queueUrl,
            MessageBody = messageBody
        };

        logger.LogInformation("Publishing message to queue {QueueUrl}. Message type: {MessageType}",
            queueUrl, typeof(T).Name);

        try
        {
            SendMessageResponse response = await amazonSqs.SendMessageAsync(sendMessage, cancellationToken);

            logger.LogInformation(
                "Message published successfully. MessageId: {MessageId}, QueueUrl: {QueueUrl}",
                response.MessageId,
                queueUrl
            );

            return response;
        }
        catch (AmazonSQSException ex)
        {
            logger.LogError(ex,
                "AWS SQS error while publishing message to queue {QueueUrl}. Error Code: {ErrorCode}",
                queueUrl,
                ex.ErrorCode
            );
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while publishing message to queue {QueueUrl}",
                queueUrl
            );
            return null;
        }
    }

    public async Task<IReadOnlyCollection<SendMessageBatchResponse>> PublishBatchAsync<T>(
        IEnumerable<T> messages,
        string queueUrl,
        CancellationToken cancellationToken = default)
        where T : class
    {
        List<SendMessageBatchResponse> responses = [];

        foreach (T[] batch in messages.Chunk(10))
        {
            List<SendMessageBatchRequestEntry> entries;

            try
            {
                entries = batch
                    .Index()
                    .Select(message => new SendMessageBatchRequestEntry
                    {
                        Id = message.Index.ToString(),
                        MessageBody = JsonSerializer.Serialize(message.Item)
                    })
                    .ToList();
            }
            catch (JsonException ex)
            {
                logger.LogError(ex,
                    "Failed to serialize messages of type {MessageType}",
                    typeof(T).Name
                );
                return ReadOnlyCollection<SendMessageBatchResponse>.Empty;
            }

            SendMessageBatchRequest request = new()
            {
                QueueUrl = queueUrl,
                Entries = entries
            };

            logger.LogInformation(
                "Publishing batch of {count} messages to queue {QueueUrl}",
                entries.Count,
                queueUrl
            );

            SendMessageBatchResponse response;

            try
            {
                response = await amazonSqs.SendMessageBatchAsync(request, cancellationToken);
            }
            catch (AmazonSQSException ex)
            {
                logger.LogError(ex,
                    "AWS SQS error while publishing batch to queue {QueueUrl}. Error Code: {errorCode}",
                    queueUrl,
                    ex.ErrorCode
                );
                return ReadOnlyCollection<SendMessageBatchResponse>.Empty;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Unexpected error while publishing batch to queue {QueueUrl}",
                    queueUrl
                );
                return ReadOnlyCollection<SendMessageBatchResponse>.Empty;
            }

            responses.Add(response);

            if (response.Failed != null && response.Failed.Count > 0)
            {
                logger.LogWarning(
                    "{failedCount} messages failed to publish to queue {QueueUrl}",
                    response.Failed.Count,
                    queueUrl
                );
            }
        }

        return responses;
    }
}