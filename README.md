# Pixel and Storage Service with Docker

This project contains two services, Pixel and Storage, that can be run in Docker containers. Additionally, there's an example of how to start a standalone Kafka service to support the Pixel service.

## Pixel Service
To build the Docker image for the Pixel service, use the following command:

```bash
docker build -t pixel -f ./PixelService/Pixel/Dockerfile .
docker run -p 8001:80 pixel
```

## Pixel Service
```bash
docker build -t storage -f ./StorageService/Storage/Dockerfile .
docker run -p 8002:80 storage
```

## Kafka
```bash
docker run -p 9092:9092 --name kafka-server wurstmeister/kafka:latest
```
