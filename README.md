# RedisProtobuf_Cache
Blazor App that includes caching to Redis, using Protobufs.

Just clone the repo and Run the application.

First you must have docker setup in your PC.

Next in the command prompt execute ->
docker run --name my-redis -p 5002:6379 -d redis

docker exec -it my-redis sh

redis-cli

//To check if it is working

ping

scan 0

Run the Application

Exe -> scan 0 in the cmd you will see the key getting created.

hgetall <key>

