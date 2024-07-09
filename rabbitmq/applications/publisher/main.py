# Study first the consumer code before this one.
# Turn on the consumer and see the console output of it; it will change based on the routing key you provide.
# Turn on this publisher using uvicorn main:app --host 0.0.0.0 --port 81
# Example call:
# curl -X POST "http://localhost:81/publish/" -H "Content-Type: application/json" -d '{"routing_key": "stock.nasdaq.FACE", "message": "AAPL: 150.00"}'
# The above will make the call channel.basic_publish(exchange='market_topic', routing_key='stock.nasdaq.AAPL', body='AAPL: 150.00')

import time
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import os
import pika

app = FastAPI()
COMMS_PORTFOWARDPORT_HOST_MAP = [('5672', 'rabbit@rabbitproper-rabbitmq-0.rabbitproper-rabbitmq-headless.rabbitproper.svc.cluster.local'),
               ('5673', 'rabbit@rabbitproper-rabbitmq-1.rabbitproper-rabbitmq-headless.rabbitproper.svc.cluster.local'),
               ('5674', 'rabbit@rabbitproper-rabbitmq-2.rabbitproper-rabbitmq-headless.rabbitproper.svc.cluster.local')
               ]

class Message(BaseModel):
    routing_key: str
    message: str

def publish_message(comms_port: str, routing_key:str, message: Message):
    rabbit_host = os.getenv('RABBIT_HOST', 'localhost')
    rabbit_port = int(os.getenv('RABBIT_PORT', comms_port))
    rabbit_user = os.getenv('RABBIT_USERNAME', 'guest')
    rabbit_password = os.getenv('RABBIT_PASSWORD', 'guest')

    credentials = pika.PlainCredentials(rabbit_user, rabbit_password)
    parameters = pika.ConnectionParameters(host=rabbit_host, port=rabbit_port, credentials=credentials)
    connection = pika.BlockingConnection(parameters)
    channel = connection.channel() 
    
    channel.basic_publish(
        exchange='market_topic',
        routing_key=routing_key,
        body=message,
        properties=pika.BasicProperties(
            delivery_mode=2,  # make message persistent
        )
    )
    connection.close()  # make sure network buffers are flushed and message is delivered to RabbitMQ

@app.post("/publish/")
async def publish(msg: Message):
    while True:
        for comms_port, hostname in COMMS_PORTFOWARDPORT_HOST_MAP:
            try:
                print(f"Publishing to host {hostname}")
                publish_message(comms_port, msg.routing_key, msg.message)
                return {"status": "Message published successfully"}
            except Exception as e:
                print(e)
                sleep = 3
                print(f"Unable to publish to host {hostname}, sleeping for {sleep} secs before trying next host")
                time.sleep(3)