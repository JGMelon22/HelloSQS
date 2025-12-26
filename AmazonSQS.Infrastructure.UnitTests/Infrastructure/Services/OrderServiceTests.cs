using Amazon.SQS;
using Amazon.SQS.Model;
using AmazonSQS.Core.Domains.DTOs.Requests;
using AmazonSQS.Core.Domains.Entities;
using AmazonSQS.Infrastructure.Configuration;
using AmazonSQS.Infrastructure.Interfaces.Services;
using AmazonSQS.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AmazonSQS.Infrastructure.UnitTests.Infrastructure.Services;

public class OrderServiceTests
{
    private ISqsMessagePublisher _sqsMessagePublisher;
    private IOptions<SqsOptions> _sqsOptions;
    private ILogger<OrderService> _logger;
    private OrderService _sut;

    [SetUp]
    public void SetUp()
    {
        _sqsMessagePublisher = Substitute.For<ISqsMessagePublisher>();
        _logger = Substitute.For<ILogger<OrderService>>();

        SqsOptions sqsOptions = new()
        {
            OrderCreatedQueueUrl = "http://localhost:9324/order-created"
        };

        _sqsOptions = Options.Create(sqsOptions);
        _sut = new OrderService(_sqsMessagePublisher, _sqsOptions, _logger);
    }

    [Test]
    public async Task Should_ShouldPublishMessageAndReturnResponse_When_CreateOrderAsync()
    {
        // Arrange
        OrderCreatedEventRequest request = new(CustomerId: Guid.Parse("019b5bf8-2e08-7e9d-b671-56508fc6298b"));
        SendMessageResponse response = new()
        {
            MessageId = "019b5bf9-b25b-7441-8e83-8267a6db1639",
            HttpStatusCode = System.Net.HttpStatusCode.OK
        };

        _sqsMessagePublisher
            .PublishAsync(Arg.Any<OrderCreatedEvent>(), _sqsOptions.Value.OrderCreatedQueueUrl)
            .Returns(response);

        // Act
        SendMessageResponse result = await _sut.CreateOrderAsync(request);

        // Assert
        Assert.That(result, Is.EqualTo(response));
        await _sqsMessagePublisher.Received(1).PublishAsync(
            Arg.Is<OrderCreatedEvent>(e => e.CustomerId == request.CustomerId),
            _sqsOptions.Value.OrderCreatedQueueUrl
            );
    }

    [Test]
    public async Task Should_ThrowException_When_PublishAsyncFail()
    {
        // Arrange
        OrderCreatedEventRequest request = new(CustomerId: Guid.Parse("019b5c24-2f93-7c69-8295-ea48caa06069"));

        AmazonSQSException amazonSQSException = new("Failed to publish message to SQS");

        _sqsMessagePublisher
            .PublishAsync(Arg.Any<OrderCreatedEvent>(), _sqsOptions.Value.OrderCreatedQueueUrl)
            .Throws(amazonSQSException);

        //Act and Assert
        AmazonSQSException exception = Assert.ThrowsAsync<AmazonSQSException>(async () => await _sut.CreateOrderAsync(request));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Failed to publish message to SQS"));
    }
}
