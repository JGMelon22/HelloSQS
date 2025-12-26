namespace AmazonSQS.Core.Domains.DTOs.Responses;

public record OrderCreatedEventResponse
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime CreatedDate { get; init; }
}
