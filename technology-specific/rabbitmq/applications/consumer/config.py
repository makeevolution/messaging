import os
RABBITMQ_HOST = os.getenv('RABBIT_HOST', 'localhost')
RABBITMQ_PORT = int(os.getenv('RABBIT_PORT', '5672'))
RABBITMQ_USER = os.getenv('RABBIT_USERNAME', 'guest')
RABBITMQ_PASSWORD = os.getenv('RABBIT_PASSWORD', 'guest')
RABBITMQ_QUEUE = 'myQueue'