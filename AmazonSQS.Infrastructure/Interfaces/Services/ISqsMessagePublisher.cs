namespace AmazonSQS.Infrastructure.Interfaces.Services;

public interface ISqsMessagePublisher
{
    Task PublishAsync<T>(T message, string queueUrl, CancellationToken cancellationToken = default) where T : class;
}