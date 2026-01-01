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

namespace AmazonSQS.Infrastructure.UnitTests.Services;

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
    public async Task Should_PublishMessageAndReturnResponse_When_CreateOrderAsync()
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
    public void Should_ThrowException_When_PublishAsyncFail()
    {
        // Arrange
        OrderCreatedEventRequest request = new(CustomerId: Guid.Parse("019b5c24-2f93-7c69-8295-ea48caa06069"));

        AmazonSQSException amazonSqsException = new("Failed to publish message to SQS");

        _sqsMessagePublisher
            .PublishAsync(Arg.Any<OrderCreatedEvent>(), _sqsOptions.Value.OrderCreatedQueueUrl)
            .Throws(amazonSqsException);

        //Act and Assert
        AmazonSQSException exception =
            Assert.ThrowsAsync<AmazonSQSException>(async () => await _sut.CreateOrderAsync(request));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Failed to publish message to SQS"));
    }

    [Test]
    public async Task Should_PublishMessagesInBatchAndReturnResponses_When_CreateOrderBatchAsync()
    {
        // Arrange
        ICollection<OrderCreatedEventRequest> requests =
        [
            new(CustomerId: Guid.Parse("0190f2c8-3c5a-7b21-9f4d-6a2e4c8b91a7")),
            new(CustomerId: Guid.Parse("0190f2c8-3c5b-7e9a-8c31-b7d5a4e0f2c9")),
            new(CustomerId: Guid.Parse("019b79cb-c7ac-7569-8c68-e92b228a2c9c"))
        ];

        SendMessageBatchResponse response = new()
        {
            Successful =
            [
                new()
                {
                    Id = "0190f2c9-8a10-7c3f-a912-4e6b0d5f21a8",
                    MessageId = "0190f2c9-8a11-7f82-b3c4-91d6e2a0587f"
                },
                new()
                {
                    Id = "0190f2ca-d4e2-7a19-9c3b-5f8e21a6d407",
                    MessageId = "0190f2ca-d4e3-7f6c-b142-0a9e5c3d8f21"
                },
                new()
                {
                    Id = "0190f2cb-2f10-7b4a-8d21-6c9e5a1f3048",
                    MessageId = "0190f2cb-2f11-7e93-b0c4-1a7d9f5e8260"
                }
            ],
            Failed = []
        };

        _sqsMessagePublisher
            .PublishBatchAsync(
                Arg.Any<IEnumerable<OrderCreatedEvent>>(),
                _sqsOptions.Value.OrderCreatedQueueUrl)
            .Returns(new List<SendMessageBatchResponse> { response });

        // Act
        IReadOnlyCollection<SendMessageBatchResponse> result = await _sut.CreateOrderBatchAsync(requests);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));

        Assert.That(result.First().Successful.Count, Is.EqualTo(3));
        Assert.That(result.First().Failed, Is.Empty);

        // Assert.That(result, Is.EqualTo(response));

        await _sqsMessagePublisher.Received(1).PublishBatchAsync(
            Arg.Is<IEnumerable<OrderCreatedEvent>>(e =>
                e.Count() == 3),
            _sqsOptions.Value.OrderCreatedQueueUrl
        );
    }

    [Test]
    public void Should_ThrowException_When_PublishBatchAsyncFail()
    {
        // Arrange
        ICollection<OrderCreatedEventRequest> requests =
        [
            new(CustomerId: Guid.Parse("019b5c24-2f93-7c69-8295-ea48caa06069")),
            new(CustomerId: Guid.Parse("019b5c24-2f93-7c69-8295-ea48caa06070"))
        ];

        AmazonSQSException amazonSqsException = new("Failed to publish batch messages to SQS");

        _sqsMessagePublisher
            .PublishBatchAsync(
                Arg.Any<IEnumerable<OrderCreatedEvent>>(),
                _sqsOptions.Value.OrderCreatedQueueUrl
            )
            .Throws(amazonSqsException);

        // Act and Assert
        AmazonSQSException exception =
            Assert.ThrowsAsync<AmazonSQSException>(async () => await _sut.CreateOrderBatchAsync(requests));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Failed to publish batch messages to SQS"));
    }
}