## overview
![[Drawing 2024-12-23 09.28.39.excalidraw  | 800x400]]

## storage-first API:

![[Drawing 2024-12-23 10.14.03.excalidraw | 800x400]]
## Frontend updates in an async world 

- So the backend updates are done through outbox pattern.
- How about the frontend? How do we update the frontend periodically without user having to refresh the page?
- Polling vs. WebSockets
	- Polling: Frontend periodically calls a backend API to update its contents
	- Websockets: Frontend connects to a websocket to backend where the backend can send a notification to the frontend after an event is published, and the frontend can display that notification (min 3:35 and 8:40)

## Thin Events and Smarter callbacks

- Basically in this vid, it shows how events should be thin, and more details can be obtained by e.g. making a callback to the service publishing the event through some other channel that has strong type definitions for their interfaces e.g. GRPC
- In min 1:50, we see the details of the orderConfirmedEvent, and how that event only contains the eventId (i.e. thin) 
- The consumer of this event i.e. the kitchen service min 3:14, will need more info on the order to get the correct recipe (for example)! 
- Recipes are highly complex (contain a lot of fields of strings, enums etc.) and sending it as an event data along with the order id can cause breaking things
- Minute 4:25 and 5:38 shows how we can get the details of the order by making a callback through GRPC to the order service using the event ID
- This ensures details are transmitted to the outside world through a mechanism that is strongly typed, and events (which inherently don't support such typing) can remain lightweight and dont introduce potential breaking changes

## Idempotency

- Same message published multiple times must be handled only once by the consumer
	- Important e.g. to prevent the same event being handled twice e.g. prevent double charging of customer funds!
- To achieve this:
	- Publisher needs to have an event ID in its header min 2:42
	- When consumer receives the event, grab the event ID and check if it has been processed before (min 4:41) through a cache mechanism (min 4:58 to 5:10) e.g. the IDistributedCache of ASP.NET Core
	- If not yet been processed, process it, then register it in cache (min 6:35) with some ttl min 6:47
		- Important to process it first, especially doing this in a bare except catch all (min 6:39!)
		- Length of ttl is depending on business logic; evaluate your own business case

## Versioning events and evolvability
- If we want to add e.g. new fields to our events, how should we do it (min 1:10)?
- Best way is to:
	- Add V1 to all our current events (version 1)
	- Make a new version V2 with the new field (min 2:28)
	- Create a publisher for this V2 event min (min 3:10)
	- In the method that published V1, also add the V2 event publisher to publish V2 events (min 4:00)
	- The outbox will pickup these 2 versions of the same event and publish them
- Set a deprecation date for your v1 event (min 5:00)!
	- Once the date is hit then can easily remove this publisher of V1 events from the cache 
- Evolvability: Your RabbitMQ routing keys should have event version at the end e.g. <eventname>.<eventversion> so subscribers can listen to different versions of that event min 6:10!

## Handling failures











