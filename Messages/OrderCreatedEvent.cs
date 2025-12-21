namespace Messages;

public class OrderCreatedEvent : IEvent
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedDate { get; set; }

    public OrderCreatedEvent(Guid orderId, Guid customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
        CreatedDate = DateTime.UtcNow;
    }
}
