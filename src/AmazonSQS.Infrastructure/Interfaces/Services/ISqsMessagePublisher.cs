using Amazon.SQS.Model;

namespace AmazonSQS.Infrastructure.Interfaces.Services;

public interface ISqsMessagePublisher
{
    Task<SendMessageResponse?> PublishAsync<T>(T message, string queueUrl, CancellationToken cancellationToken = default) where T : class;
    Task<IReadOnlyCollection<SendMessageBatchResponse>> PublishBatchAsync<T>(IEnumerable<T> messages, string queueUrl, CancellationToken cancellationToken = default) where T : class;
}