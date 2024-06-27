import os
RABBITMQ_HOST = os.getenv('RABBIT_HOST', 'localhost')
RABBITMQ_PORT = int(os.getenv('RABBIT_PORT', '5672'))
RABBITMQ_USER = os.getenv('RABBIT_USERNAME', 'guest')
RABBITMQ_PASSWORD = os.getenv('RABBIT_PASSWORD', 'guest')
RABBITMQ_QUEUE = 'myQueue'

import pika
import threading

def connect_to_rabbitmq():
    credentials = pika.PlainCredentials(RABBITMQ_USER, RABBITMQ_PASSWORD)
    parameters = pika.ConnectionParameters(host=RABBITMQ_HOST, port=RABBITMQ_PORT, credentials=credentials)
    connection = pika.BlockingConnection(parameters)
    return connection

def consume():
    connection = connect_to_rabbitmq()
    channel = connection.channel()
    channel.queue_declare(queue=RABBITMQ_QUEUE, durable=True)

    def callback(ch, method, properties, body):
        print(f"Received {body}")
        # Process the message here

    channel.basic_consume(queue=RABBITMQ_QUEUE, on_message_callback=callback, auto_ack=True)

    print('Waiting for messages. To exit press CTRL+C')
    channel.start_consuming()

def start_consumer_thread():
    thread = threading.Thread(target=consume)
    thread.start()
    return thread

# app/main.py
from fastapi import FastAPI

app = FastAPI()

@app.on_event("startup")
async def startup_event():
    start_consumer_thread()

@app.get("/")
async def read_root():
    return {"Hello": "World"}

@app.get("/health")
async def health_check():
    return {"status": "healthy"}
