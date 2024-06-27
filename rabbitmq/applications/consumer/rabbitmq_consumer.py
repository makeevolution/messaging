import pika
import threading

from applications.consumer.config import RABBITMQ_HOST, RABBITMQ_PASSWORD, RABBITMQ_PORT, RABBITMQ_QUEUE, RABBITMQ_USER
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

    channel.basic_consume(queue=RABBITMQ_QUEUE, on_message_callback=callback, auto_ack=True)

    print('Waiting for messages. To exit press CTRL+C')
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