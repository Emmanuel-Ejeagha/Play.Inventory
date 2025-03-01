## **Play.Inventory MicroService**

````md
# Inventory Service

## Overview

The **Inventory Service** is a microservice responsible for managing product stock levels. It ensures that products are available for orders and tracks inventory changes.

## Tech Stack

- .NET 8.0
- ASP.NET Core Web API
- MongoDB (Database)
- Docker (Containerization)
- RabbitMQ _(messaging)_
- Swagger (API Documentation)

## Features

- Track available stock for products.
- Update stock levels when items are sold or restocked.
- Check product availability.

## Installation

### Prerequisites

- .NET 8.0 SDK
- Docker (running in containers)
- MongoDB installed and running in a container

### Clone the Repository

```sh
git clone https://github.com/Emmanuel-Ejeagha/Play.Inventory.git
cd Play.Inventory
```
````

### Run the Service Locally

```sh
dotnet run
```

### Run with Docker

```sh
docker-compose up -d
```

### API Endpoints

| Method | Endpoint                 | Description                   |
| ------ | ------------------------ | ----------------------------- |
| GET    | `/api/items`             | Get items for all products    |
| GET    | `/api/items/{productId}` | Get stock level for a product |
| POST   | `/api/items`             | Add new stock                 |
| PUT    | `/api/items/{productId}` | Update stock level            |
| DELETE | `/api/items/{productId}` | Remove stock entry            |

### Environment Variables

| Variable                  | Description                 |
| ------------------------- | --------------------------- |
| `MONGO_CONNECTION_STRING` | MongoDB connection string   |
| `SERVICE_PORT`            | Port number for the service |

## Contributing

1. Fork the repository.
2. Create a new feature branch.
3. Commit and push your changes.
4. Submit a pull request.

## License

MIT License
