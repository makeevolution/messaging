# Example call:
# curl -X POST "http://localhost:8000/publish" -H "Content-Type: application/json" -d '{"routing_key": "my_routing_key", "message": "Hello, RabbitMQ!"}'

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import os
import pika

app = FastAPI()

class Message(BaseModel):
    routing_key: str
    message: str

def publish_message(routing_key:str, message: Message):
    rabbit_host = os.getenv('RABBIT_HOST', 'localhost')
    rabbit_port = int(os.getenv('RABBIT_PORT', '5672'))
    rabbit_user = os.getenv('RABBIT_USERNAME', 'guest')
    rabbit_password = os.getenv('RABBIT_PASSWORD', 'guest')

    credentials = pika.PlainCredentials(rabbit_user, rabbit_password)
    parameters = pika.ConnectionParameters(host=rabbit_host, port=rabbit_port, credentials=credentials)
    connection = pika.BlockingConnection(parameters)
        channel = connection.channel() 
        
        channel.basic_publish(
            exchange='',
            routing_key=routing_key,  # this is the queue name to publish to
            body=message,
            properties=pika.BasicProperties(
                delivery_mode=2,  # make message persistent
            )
        )
        connection.close()  # make sure network buffers are flushed and message is delivered to RabbitMQ

@app.post("/publish/")
async def publish(msg: Message):
    try:
        publish_message(msg.routing_key, msg.message)
        return {"status": "Message published successfully"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
