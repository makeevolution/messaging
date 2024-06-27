from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import os
import pika

app = FastAPI()

class Message(BaseModel):
    content: str

@app.post("/publish/")
def publish_message(message: Message):
    rabbit_host = os.getenv('RABBIT_HOST', 'localhost')
    rabbit_port = int(os.getenv('RABBIT_PORT', '5672'))
    rabbit_user = os.getenv('RABBIT_USERNAME', 'guest')
    rabbit_password = os.getenv('RABBIT_PASSWORD', 'guest')

    credentials = pika.PlainCredentials(rabbit_user, rabbit_password)
    parameters = pika.ConnectionParameters(host=rabbit_host, port=rabbit_port, credentials=credentials)

    try:
        print("got here")
        connection = pika.BlockingConnection(parameters)
        channel = connection.channel() 
        # This is like get_or_create of Django. If the queue exists already it won't be created.
        # Otherwise, it will be created.
        channel.queue_declare(queue='myQueue', durable=True)
        print("gothere 2")
        channel.basic_publish(
            exchange='',
            routing_key='myQueue',  # this is the queue name to publish to
            body=message.content,
            properties=pika.BasicProperties(
                delivery_mode=2,  # make message persistent
            )
        )
        print("publishoke")
        connection.close()  # make sure network buffers are flushed and message is delivered to RabbitMQ
        return {"status": "Message published successfully"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
