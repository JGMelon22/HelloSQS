using AmazonSQS.Core.Domains.DTOs.Requests;

namespace AmazonSQS.Infrastructure.Interfaces.Services;

public interface IOrderService
{
    Task<Guid> CreateOrderAsync(OrderCreatedEventRequest request, CancellationToken cancellationToken = default);
}
