# app/main.py
from fastapi import FastAPI
from rabbitmq_consumer import start_consumer_thread, stop_consumer_thread

app = FastAPI()

# So, the server, upon startup, will create a separate thread that will communicate, monitor
# and consume messages given on a specific topic.
# In Django, we can I think put this in ready() function
@app.on_event("startup")
async def startup_event():
    global consumer_thread
    consumer_thread = start_consumer_thread()

@app.on_event("shutdown")
async def shutdown_event():
    stop_consumer_thread()

# Make some fake endpoints. For the purposes of RabbitMQ demo these don't really
# matter
@app.get("/")
async def read_root():
    return {"Hello": "World"}

@app.get("/health")
async def health_check():
    return {"status": "healthy"}
