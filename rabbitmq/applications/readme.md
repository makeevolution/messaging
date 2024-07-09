This is a sample app that consumes and publish to a rabbitMQ cluster with a quorum queue enabled.
Configure it yourself, dont forget to make venv!
Install requirements, and run the fastapi servers e.g. 'uvicorn main:app --host 0.0.0.0 --port 80'
Don't forget to `kubectl port-forward` the RabbitMQ nodes:

- `kubectl port-forward pods/rabbitproper-rabbitmq-2 -n rabbitproper 15672` for UI. 

- For comms, to demo out a High Available cluster, run 
`while true; do kubectl port-forward pod/rabbitproper-rabbitmq-0 -n rabbitproper 5672:5672; done`
`while true; do kubectl port-forward pod/rabbitproper-rabbitmq-1 -n rabbitproper 5673:5672; done`
`while true; do kubectl port-forward pod/rabbitproper-rabbitmq-2 -n rabbitproper 5674:5672; done`
and then kill the rabbitmq-0 one using kubectl delete, and watch in the UI as the queues get assigned/lead to other nodes automatically.
Also try out and realize that, even if the queues are assigned/lead in node 0 (for example), you can
still consume that queue from node 1 or 2!


The app is an example of topic exchange, following the tutorial `https://www.rabbitmq.com/tutorials/tutorial-five-python` but adapting it for use in a FastAPI context.