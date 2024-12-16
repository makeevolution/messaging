import sys
import time
import pika
import threading

from config import RABBITMQ_HOST, RABBITMQ_PASSWORD, RABBITMQ_PORT, RABBITMQ_USER
stop_event = threading.Event()
COMMS_PORTFOWARDPORT_HOST_MAP = [('5672', 'rabbit@rabbitproper-rabbitmq-0.rabbitproper-rabbitmq-headless.rabbitproper.svc.cluster.local'),
               ('5673', 'rabbit@rabbitproper-rabbitmq-1.rabbitproper-rabbitmq-headless.rabbitproper.svc.cluster.local'),
               ('5674', 'rabbit@rabbitproper-rabbitmq-2.rabbitproper-rabbitmq-headless.rabbitproper.svc.cluster.local')
               ]

def connect_to_rabbitmq(comms_port):
    credentials = pika.PlainCredentials(RABBITMQ_USER, RABBITMQ_PASSWORD)
    parameters = pika.ConnectionParameters(host=RABBITMQ_HOST, port=comms_port, credentials=credentials)
    connection = pika.BlockingConnection(parameters)
    channel = connection.channel()
    return channel

def setup_queues_in_channel(channel):
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
    # The producer would send a routing_key and message combo like these:
    # channel.basic_publish(exchange='market_topic', routing_key='stock.nasdaq.AAPL', body='AAPL: 150.00')
    # channel.basic_publish(exchange='market_topic', routing_key='stock.nasdaq.GOOG', body='GOOG: 2800.00')
    # channel.basic_publish(exchange='market_topic', routing_key='stock.nyse.GE', body='GE: 100.00')
    # channel.basic_publish(exchange='market_topic', routing_key='stock.nyse.TSLA', body='TSLA: 800.00')
    # channel.basic_publish(exchange='market_topic', routing_key='stock.us.TSLA', body='TSLA: 800.00')
    # channel.basic_publish(exchange='market_topic', routing_key='bond.us.TBOND10', body='US 10-Year Treasury Bond: 1.50%')
    # channel.basic_publish(exchange='market_topic', routing_key='bond.eu.BOND30', body='EU 30

    # Define some queues that listens to predefined specific routing keys (i.e. desired info)
    routing_key_stock_nasdaq_AAPL = "stock.nasdaq.APPL"
    channel.queue_declare(queue=routing_key_stock_nasdaq_AAPL, arguments={'x-queue-type': 'quorum'}, durable=True)  # queue for Specific Stock (e.g., AAPL); note: the name can be anything, here made like this for clarity
    channel.queue_bind(queue=routing_key_stock_nasdaq_AAPL, exchange="market_topic", routing_key=routing_key_stock_nasdaq_AAPL)
    # This is the meat. When there is a message put to the queue, this 
    # function will trigger and do some work.
    def callback_queue_stock_nasdaq_AAPL(ch, method, properties, body):
        print(f"Queue with name {routing_key_stock_nasdaq_AAPL} received message: {body} from routing key {method.routing_key}")
    # This is also the meat. This function will monitor the queue and call the callback
    # function we defined above. 
    # auto_ack is super important to understand, please read: 
    # https://www.rabbitmq.com/tutorials/tutorial-two-python#message-acknowledgment
    channel.basic_consume(queue=routing_key_stock_nasdaq_AAPL, on_message_callback=callback_queue_stock_nasdaq_AAPL, auto_ack=True)

    routing_key_stock_nasdaq = "stock.nasdaq.#"
    channel.queue_declare(queue=routing_key_stock_nasdaq, arguments={'x-queue-type': 'quorum'}, durable=True)  # queue for entire nasdaq stock market
    channel.queue_bind(queue=routing_key_stock_nasdaq, exchange="market_topic", routing_key=routing_key_stock_nasdaq)  # So any stock, or even no stock, in the nasdaq, will be listened to
    def callback_queue_stock_nasdaq(ch, method, properties, body):
        print(f"Queue with name {routing_key_stock_nasdaq} received message: {body} that was sent with routing key: {method.routing_key}")
    channel.basic_consume(queue=routing_key_stock_nasdaq, on_message_callback=callback_queue_stock_nasdaq, auto_ack=True)

    routing_key_stock = "stock.#"
    channel.queue_declare(queue=routing_key_stock, arguments={'x-queue-type': 'quorum'}, durable=True)  # queue for all stock markets
    channel.queue_bind(queue=routing_key_stock, exchange="market_topic", routing_key=routing_key_stock)  # So any stock, or even no stock, in any market, will be listened to
    def callback_queue_stock(ch, method, properties, body):
        print(f"Queue with name {routing_key_stock} received message: {body} that was sent with routing key: {method.routing_key}")
    channel.basic_consume(queue=routing_key_stock, on_message_callback=callback_queue_stock, auto_ack=True)

    routing_key_any_instrument_us_any_ticker = "*.us.*"
    channel.queue_declare(queue=routing_key_any_instrument_us_any_ticker, arguments={'x-queue-type': 'quorum'}, durable=True)  # queue for any instruments with any ticker in the us market
    channel.queue_bind(queue=routing_key_any_instrument_us_any_ticker, exchange="market_topic", routing_key=routing_key_any_instrument_us_any_ticker)  # So any instrument with any ticker, as long as it is traded in the us market, will be listened to.
    def callback_any_instrument_us_any_ticker(ch, method, properties, body):
        print(f"Queue with name {routing_key_any_instrument_us_any_ticker} received message: {body} that was sent with routing key: {method.routing_key}")
    channel.basic_consume(queue=routing_key_any_instrument_us_any_ticker, on_message_callback=callback_any_instrument_us_any_ticker, auto_ack=True)

    routing_key_bond = "bond.#"
    channel.queue_declare(queue=routing_key_bond, arguments={'x-queue-type': 'quorum'}, durable=True)  # queue for all bond markets
    channel.queue_bind(queue=routing_key_bond, exchange="market_topic", routing_key=routing_key_bond)  # So any bond, or even no bond, will be listened to
    def callback_queue_bond(ch, method, properties, body):
        print(f"Queue with name {routing_key_bond} received message: {body} that was sent with routing key: {method.routing_key}")
    channel.basic_consume(queue=routing_key_bond, on_message_callback=callback_queue_bond, auto_ack=True)

    # So from the example above, the general practice is that the publisher is the one that is as specific as possible, and the queues are the ones that are flexible.
    # Publishers can ofc use routing keys with wildcards (* and #) to specify general patterns, 
    # but each message is typically published with a specific routing key that fits its intended category or topic.

def consume():
    # This bit of code will make it so that this function does not terminate, until the stop_event
    # is set (which will be set when the server is shutting down). This will then ensure this consumer
    # thread is terminated properly and resources are free-ed.
    while True:
        for comms_port, hostname in COMMS_PORTFOWARDPORT_HOST_MAP:
            try:
                channel = connect_to_rabbitmq(comms_port)
                setup_queues_in_channel(channel)
                print(f'Connected to host {hostname}; waiting for messages. To exit press CTRL+C')
                while not stop_event.is_set():
                    channel.start_consuming()

            except Exception as e:
                print(e)
                sleep = 3
                print(f"Connection to host {hostname} is broken!, sleeping for {sleep} secs before trying next host")
                time.sleep(3)

def start_consumer_thread():
    thread = threading.Thread(target=consume)
    thread.daemon = True  # So that when the server shuts down (i.e. the parent shuts down), this consumer thread is also shut down
                          # This is not good practice though since the thread is killed abruptly and it cannot call
                          # channel.close() and connection.close()... Need to find a better way
    thread.start()
    return thread

def stop_consumer_thread():
    print("stopping server")
    stop_event.set()