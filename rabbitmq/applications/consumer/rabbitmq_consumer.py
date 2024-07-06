import pika
import threading

from config import RABBITMQ_HOST, RABBITMQ_PASSWORD, RABBITMQ_PORT, RABBITMQ_QUEUE, RABBITMQ_USER
stop_event = threading.Event()

def connect_to_rabbitmq():
    credentials = pika.PlainCredentials(RABBITMQ_USER, RABBITMQ_PASSWORD)
    parameters = pika.ConnectionParameters(host=RABBITMQ_HOST, port=RABBITMQ_PORT, credentials=credentials)
    connection = pika.BlockingConnection(parameters)
    return connection

def consume():
    connection = connect_to_rabbitmq()
    channel = connection.channel()

    # Declare/connect to an (maybe existing) exchange of type topic with name topic_logs
    # P.S. we need to think about where we can store all the exchange and queue names in one central location...
    channel.exchange_declare(exchange='market_topic', exchange_type='topic')

    # Now, let's bind the exchange to a queue with a binding key.
    # Since we use an topic exchange, we can bind keys with a regex-like pattern.
    # The pattern is a list of words, delimited by dots
    # e.g. stock.usd.nyse, nyse.vfw, etc.
    # A * can be used to substitute for exactly one word, a # for zero or more words.
    # As an example, a queue bound to a topic exchange with key *.usd.*, will only accept messages sent with a routing key e.g. a.usd.b, c.usd.d
    # and will not accept e.g. a.usd, usd.b
    # Another example, a queue bound to a topic exchange with key usd.# , will only accept messages sent with a routing key e.g. usd, usd.a, usd.b.c, usd.d.e.f
    # and will not accept e.g. a.usd
    # You can bind multiple keys to a queue an and exchange.
    # Consequently, you will have perhaps 'conflicts' in your keys, e.g. *.*.usd and a.# ; If you bound these two to a queue and an exchange, both will match a.b.usd
    # In this case, the message will be delivered to the queue only once.

    # Let's say we want to get updates for the investment market, with format instrument.market.ticker
    # The producer would send a routing_key and message combo like
    # channel.basic_publish(exchange='market_topic', routing_key='stock.nasdaq.AAPL', body='AAPL: 150.00')
    # channel.basic_publish(exchange='market_topic', routing_key='stock.nasdaq.GOOG', body='GOOG: 2800.00')
    # channel.basic_publish(exchange='market_topic', routing_key='stock.nyse.GE', body='GE: 100.00')
    # channel.basic_publish(exchange='market_topic', routing_key='stock.nyse.TSLA', body='TSLA: 800.00')
    # channel.basic_publish(exchange='market_topic', routing_key='stock.us.TSLA', body='TSLA: 800.00')
    # channel.basic_publish(exchange='market_topic', routing_key='bond.us.TBOND10', body='US 10-Year Treasury Bond: 1.50%')
    # channel.basic_publish(exchange='market_topic', routing_key='bond.eu.BOND30', body='EU 30

    # Define some queues that listens to predefined specific routing keys (i.e. desired info)
    channel.queue_declare(name='queue.stock.nasdaq.AAPL', durable=True)  # queue for Specific Stock (e.g., AAPL); note: the name can be anything, here made like this for clarity
    channel.queue_bind(exchange="market_topic", routing_key="stock.nasdaq.AAPL")

    channel.queue_declare(name='queue.stock.nasdaq', durable=True)  # queue for entire nasdaq stock market
    channel.queue_bind(exchange="market_topic", routing_key="stock.nasdaq.#")  # So any stock, or even no stock, in the nasdaq, will be listened to

    channel.queue_declare(name='queue.stock', durable=True)  # queue for all stock markets
    channel.queue_bind(exchange="market_topic", routing_key="stock.#")  # So any stock, or even no stock, in any market, will be listened to

    channel.queue_declare(name='queue.any_instrument.us.any_ticker', durable=True)  # queue for any instruments with any ticker in the us market
    channel.queue_bind(exchange="market_topic", routing_key="*.us.*")  # So any instrument with any ticker, as long as it is traded in the us market, will be listened to.

    channel.queue_declare(name='queue.bond', durable=True)  # queue for all bond markets
    channel.queue_bind(exchange="market_topic", routing_key="bond.#")  # So any bond, or even no bond, will be listened to

    channel.queue_declare(name='queue.bond', durable=True)  # queue for all bond markets
    channel.queue_bind(exchange="market_topic", routing_key="bond.#")  # So any bond, or even no bond, will be listened to


    channel.queue_declare(name='')


    # This is the meat. When there is a message put to the queue, this 
    # function will trigger and do some work. 
    # I think for our context, this can
    # be when VFM2 backend publishes a message to the queue, we call k8s to create
    # a e2e pod.
    def callback(ch, method, properties, body):
        print(f"Received {body}")
        # Execute an e2e job here (havent figured how yet...)
    
    # This is also the meat. This function will monitor the queue and call the callback
    # function we defined above. 
    # auto_ack is super important to understand, please read: 
    # https://www.rabbitmq.com/tutorials/tutorial-two-python#message-acknowledgment
    
    channel.basic_consume(queue=RABBITMQ_QUEUE, on_message_callback=callback, auto_ack=True)

    print('Waiting for messages. To exit press CTRL+C')

    # This bit of code will make it so that this function does not terminate, until the stop_event
    # is set (which will be set when the server is shutting down). This will then ensure this consumer
    # thread is terminated properly and resources are free-ed.
    while not stop_event.is_set():
        connection.process_data_events(time_limit=1)
    
    channel.close()
    connection.close()

def start_consumer_thread():
    thread = threading.Thread(target=consume)
    thread.start()
    return thread

def stop_consumer_thread():
    stop_event.set()