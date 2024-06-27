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
        channel.queue_declare(queue='publisher', durable=True)
        print("gothere 2")
        channel.basic_publish(
            exchange='',
            routing_key='publisher',
            body=message.content,
            properties=pika.BasicProperties(
                delivery_mode=2,  # make message persistent
            )
        )
        print("publishoke")
        connection.close()
        return {"status": "Message published successfully"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

