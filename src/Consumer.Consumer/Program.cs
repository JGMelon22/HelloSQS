using Amazon.SQS;
using AmazonSQS.Infrastructure.Configuration;
using Consumer.Consumer.BackgroundServices;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHostedService<OrderCreatedEventConsumer>();

        builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
        builder.Services.AddAWSService<IAmazonSQS>();

        builder.Services.Configure<SqsOptions>(
            builder.Configuration.GetSection(SqsOptions.SqsSettings));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}