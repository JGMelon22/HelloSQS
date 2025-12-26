using AmazonSQS.Core.Domains.DTOs.Requests;
using AmazonSQS.Core.Domains.Entities;
using AmazonSQS.Core.Domains.Mappings;
using AmazonSQS.Infrastructure.Configuration;
using AmazonSQS.Infrastructure.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AmazonSQS.Infrastructure.Services;

public class OrderService(
    ISqsMessagePublisher sqsMessagePublisher,
    IOptions<SqsOptions> sqsOptions,
    ILogger<OrderService> logger)
    : IOrderService
{
    private readonly SqsOptions _sqsOptions = sqsOptions.Value;

    public async Task<Guid> CreateOrderAsync(OrderCreatedEventRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Creating order for customer {customerId}",
            request.CustomerId
        );

        OrderCreatedEvent @event = request.ToDomain();

        await sqsMessagePublisher.PublishAsync(@event, _sqsOptions.OrderCreatedQueueUrl, cancellationToken);

        logger.LogInformation(
            "Order {orderId} created successfully for customer {customerId}",
            @event.OrderId,
            @event.CustomerId
        );

        return @event.OrderId;
    }
}
