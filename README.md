# AmazonSQS.Demo

This project demonstrates the implementation of a messaging system using [Amazon SQS](https://aws.amazon.com/sqs/) (Simple Queue Service) with a producer-consumer architecture. The system uses [ElasticMQ](https://github.com/softwaremill/elasticmq) to simulate SQS locally during development and testing.

---

## 🚀 Motivation

This project was created to learn and master technologies used in my current workplace, which relies on the AWS stack, along with curiosity to explore modern development patterns:

- **Amazon SQS**: Understanding asynchronous messaging and distributed systems
- **NUnit**: Learning structured and reliable unit testing
- **NSubstitute**: Mastering mocking techniques to isolate dependencies in tests
- **Options Pattern**: Applying .NET configuration patterns

---

## 🗺️ Project Structure

```
HelloSQS
├── src
│   ├── AmazonSQS.Core                      # Domain layer
│   │   └── Domains
│   │       ├── DTOs                        # Data Transfer Objects
│   │       │   ├── Requests
│   │       │   └── Responses
│   │       ├── Entities                    # Domain entities
│   │       └── Mappings                    # Object mappings
│   │
│   ├── AmazonSQS.Infrastructure            # Infrastructure layer
│   │   ├── BackgroundServices              # Processing workers
│   │   ├── Configuration                   # Configurations (Options Pattern)
│   │   ├── Interfaces
│   │   │   └── Services                    # Service contracts
│   │   └── Services                        # SQS service implementations
│   │
│   ├── Consumer.API                        # Consumer API
│   │   ├── Controllers                     # API endpoints
│   │   └── Middlewares                     # Custom middlewares
│   │
│   └── Consumer.Consumer                   # Consumer worker
│       └── Properties                      # Application properties
│
└── tests
    └── UnitTests
        └── AmazonSQS.Infrastructure.UnitTests  # Unit tests (NUnit + NSubstitute)
            └── Services                        # Service tests
```

---

## 🧰 Tech Stack

<div style="display: flex; gap: 10px;">
    <img height="32" width="32" src="https://cdn.simpleicons.org/dotnet" alt=".NET" title=".NET" />
    <img height="32" width="32" src="https://cdn.simpleicons.org/swagger" alt="Swagger" title="Swagger" />
    <img height="32" width="32" src="https://cdn.simpleicons.org/docker" alt="Docker" title="Docker" />
	<img src="https://img.shields.io/badge/AWS-%23FF9900.svg?style=for-the-badge&logo=amazon-aws&logoColor=white" alt="AWS" title="AWS" />
</div>

<br/>

- **.NET 10** – Main backend framework
- **Swagger** – Interactive API documentation
- **Amazon SQS** – Message queuing service
- **ElasticMQ** – Local Amazon SQS simulator
- **NUnit** – Unit testing framework
- **NSubstitute** – Mocking library for tests
- **Options Pattern** – .NET configuration pattern

---

## 🏗️ Architecture

The project follows **Clean Architecture** and **SOLID** principles, with a well-defined message flow:

1. The **Producer API** publishes messages to the SQS queue
2. The **Background Service** continuously monitors the queue
3. Messages are consumed, processed, and removed after success
4. In case of failure, messages return to the queue for retry

---

## ⚙️ Configuration

### Prerequisites

- .NET 10 SDK
- Docker (to run ElasticMQ)

### Local ElasticMQ

Run ElasticMQ via Docker:

```bash
docker run -d --name ElasticMQ -p 9324:9324 softwaremill/elasticmq-native
```

### Application Configuration

Configure in `appsettings.json`:

```json
{
  "AWS": {
    "Region": "us-east-1",
    "ServiceURL": "http://localhost:9324",
    "QueueUrl": "http://localhost:9324/your-queue-name"
  }
}
```

---

## 🧪 Tests

Run unit tests:

```bash
dotnet test
```

---

## 🙏 Acknowledgments

- [ElasticMQ](https://github.com/softwaremill/elasticmq) for providing an excellent local SQS simulation tool