using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Anko.PaymentsService.Adapters;
public record ChannelConfig(string ExchangeName, string QueueName, string RoutingKey, string DeadLetterExchangeName, string DeadLetterQueueName,  AsyncEventHandler<BasicDeliverEventArgs> Consumer);

public record ConfiguredChannel(IChannel Channel, AsyncEventingBasicConsumer Consumer);

// 
public class RabbitMQConnection
{
    public IConnection Connection { get; init; }

    /* 
    Ensure message bus exists, and initialize the Connection field for this connection instance
    Also ensure exchange(s) exist, otherwise create it.

    What is involved?
    1. Making the connection and configure field for this instance
    2. Making a channel using the connection
    3. Establishing/ensuring exchange exists

    So, connection -> channel -> do whatever you want
    It's like database -> table -> ... for SQL tables
    */
    public RabbitMQConnection(string hostName, string exchangeName, string deadLetterExchangeName)
    {
        var connectionRetry = 10;

        while (connectionRetry > 0)
        {
            try
            {
                Console.WriteLine($"Attempting to connect to host: {hostName}");
                var factory = new ConnectionFactory()
                {
                    HostName = hostName,
                };

                Connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

                var channel = Connection.CreateChannelAsync().GetAwaiter().GetResult();
                
                // We subscribe to two exchanges: The exchange where the order creation event will be published to, as well as
                // an exchange for dead letters.
                // A dead letter exchange is an exchange where messages that are unprocessable will be thrown to, instead of just discarded.
                // See the background worker code for more details.
                // SOME LEARNING NOTE: the GetAwaiter.GetResult below is bad code to be applied in Controllers for ASP.NET non-core due to synchronization context feature.
                // See notes in Obsidian for more information.
                channel.ExchangeDeclareAsync(exchange: exchangeName, ExchangeType.Topic, durable: true).GetAwaiter().GetResult();
                channel.ExchangeDeclareAsync(exchange: deadLetterExchangeName, ExchangeType.Direct, durable: true).GetAwaiter().GetResult();
                Console.WriteLine($"Successfully connected to host: {hostName}");
                break;
            }
            catch (BrokerUnreachableException e)
            {
                Console.WriteLine(e.Message);
                
                connectionRetry--;
                
                Task.Delay(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
            }   
        }

        if (Connection is null)
        {
            throw new EventBusConnectionException(hostName, "Unable to connect to RabbitMQ");
        }
    }

    /* Create and configure a channel that will consume messages
    Steps are:
    Use the established connection to -> create a new channel
    -> bind to the two queues -> establish an listener that will fire a handler once a message is published to the queue
    -> Return the configured channel
    */
    public async Task<ConfiguredChannel> CreateAndConfigureConsumingChannel(ChannelConfig config, CancellationToken stoppingToken)
    {
        var channel = await Connection.CreateChannelAsync(cancellationToken: stoppingToken);
        
        // Declare queues and bind using routing key 
        // Some note: Remember; consumer is king i.e. its the one that defines the routing key! 
        // The publisher only needs the routing key and publish to this exchange with that key in the message it sends, 
        // it doesn't need to know anything about queues)
        
        // First declare the queue that stores dead letters
        await channel.QueueDeclareAsync(config.DeadLetterQueueName,
            exclusive: false,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);
        
        await channel.QueueBindAsync(queue: config.DeadLetterQueueName, exchange: config.DeadLetterExchangeName, routingKey: config.RoutingKey, cancellationToken: stoppingToken);
        
        // Second, declare the queue that listens for order created events
        // Note that it has the dead letter arguments; i.e. messages that can't be handled due to an error will be put into that queue.
        // The queue is also set to quorum for durability (refer to docs for more info)
        await channel.QueueDeclareAsync(config.QueueName,
            exclusive: false,
            durable: true,
            autoDelete: false,
            arguments: new Dictionary<string, object>()
            {
                {"x-dead-letter-exchange", config.DeadLetterExchangeName},
                {"x-dead-letter-routing-key", config.RoutingKey},
                {"x-queue-type", "quorum"}
            }!,
            cancellationToken: stoppingToken);
        await channel.QueueBindAsync(queue: config.QueueName, exchange: config.ExchangeName, routingKey: config.RoutingKey, cancellationToken: stoppingToken);
        
        // Finally create an event handler i.e. a consumer
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += config.Consumer;

        // The channel is fully configured (queues established, consumer established, return the channel)
        return new ConfiguredChannel(channel, consumer);
    }
}