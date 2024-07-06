This is a sample app that consumes and publish to a rabbitMQ instance.
Configure it yourself, dont forget to make venv!
Install requirements, and run the fastapi servers e.g. 'uvicorn main:app --host 0.0.0.0 --port 80'
Don't forget to `kubectl port-forward` the RabbitMQ nodes!
`kubectl port-forward pods/rabbithostname -n rabbitproper 15672` for UI, change to 5672 so that your publishers and consumers can connect to the broker.

The app is an example of topic exchange, following the tutorial `https://www.rabbitmq.com/tutorials/tutorial-five-python` but adapting it for use in a FastAPI context.