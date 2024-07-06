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
    channel.queue_declare(queue=RABBITMQ_QUEUE, durable=True)

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