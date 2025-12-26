using Amazon.SQS;
using AmazonSQS.Infrastructure.Configuration;
using AmazonSQS.Infrastructure.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSQS>();

builder.Services.Configure<SqsOptions>(
    builder.Configuration.GetSection(SqsOptions.SqsSettings));

builder.Services.AddScoped<ISqsMessagePublisher, ISqsMessagePublisher>();

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
