using Amazon.SQS.Model;
using AmazonSQS.Core.Domains.DTOs.Requests;

namespace AmazonSQS.Infrastructure.Interfaces.Services;

public interface IOrderService
{
    Task<SendMessageResponse> CreateOrderAsync(OrderCreatedEventRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SendMessageBatchResponse>> CreateOrderBatchAsync(ICollection<OrderCreatedEventRequest> request, CancellationToken cancellationToken = default);
}
