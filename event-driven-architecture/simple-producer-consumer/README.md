Experiment on event driven architecture and OpenTelemetry in .NET

Setup:
- Install .NET 9 sdk
- Install Docker
- Start postgres and rabbitmq by `docker compose up -d`
- Then the usual `dotnet build` and `dotnet run`

Running:
- Access swagger of the producer under `localhost:5093/swagger`
- Make a POST request to `orders` endpoint, and see the consumer logging the event in its logs; also see the producer's background worker processing the event using the outbox pattern
  - Note that the endpoint is rigged; if you set `customerId` to be `error`, the consumer will throw an exception. This is to simulate dead letter queueing concept.

## Learning points
- The flow of Event Driven Architecture is:
  - API endpoint takes in Handler -> Handler takes in Repository -> Repository saves/reads to/from DB and saves events to the outbox -> Background worker will pick up outbox events and publish it -> consumer picks up msg from messaging bus, and processes it
- In rabbitMQ:
  - CUSTOMER IS KING, this means:
  - An exchange is bound to a queue by a routing key set by the consumer (see `RabbitMQConnection.cs` of consumer)
  - The producer only cares about routing keys and exchanges (see `RabbitMQConnection.cs` of producer)
    - It's job is to publish to an exchange with that routing key in its header
  - Producer doesn't care/know anything/have to do anything with queues!
  - In this experiment, the routing key is `order-created` (hardcoded). But this shouldn't be the case; best put it in configuration or inject through environment variables. And remember; this is dictated by the consumer!
- Use the outbox pattern to publish messages to the event bus
- Each event published must adhere to the CloudEvents specification
- Dead Letter Queue: Queue to store messages that the consumer cannot process
- OpenTelemetry: See code in `shared` to understand how the app is instrumented with OTEL
  - In a nutshell, consumers and producers is instrumented using .NET Activity. 
  - OTEL listens to the activity through the code in `shared`
  - And it exports the activity (e.g. AddEvent, AddException, AddTag, etc.) in OTEL format to Jaeger