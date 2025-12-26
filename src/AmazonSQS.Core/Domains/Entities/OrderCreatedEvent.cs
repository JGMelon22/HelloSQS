namespace AmazonSQS.Core.Domains.Entities;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedDate { get; set; }

    public OrderCreatedEvent(Guid customerId)
    {
        OrderId = Guid.CreateVersion7();
        CustomerId = customerId;
        CreatedDate = DateTime.UtcNow;
    }
}
