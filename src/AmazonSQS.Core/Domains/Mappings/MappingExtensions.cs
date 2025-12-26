using AmazonSQS.Core.Domains.DTOs.Requests;
using AmazonSQS.Core.Domains.DTOs.Responses;
using AmazonSQS.Core.Domains.Entities;

namespace AmazonSQS.Core.Domains.Mappings;

public static class MappingExtensions
{
    public static OrderCreatedEvent ToDomain(this OrderCreatedEventRequest orderCreatedEvent)
        => new(customerId: orderCreatedEvent.CustomerId);

    public static OrderCreatedEventResponse ToResponse(this OrderCreatedEvent orderCreatedEvent)
        => new()
        {
            OrderId = orderCreatedEvent.OrderId,
            CustomerId = orderCreatedEvent.CustomerId,
            CreatedDate = orderCreatedEvent.CreatedDate
        };
}
