namespace AmazonSQS.Infrastructure.Configuration;

public class SqsOptions
{
    public const string SqsSettings = "SqsSettings";

    public string OrderCreatedQueueUrl { get; set; } = string.Empty;
}
