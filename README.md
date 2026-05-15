# NATS.Net Base Pub/Sub Demo

A lightweight, single process .NET console application demonstrating the core Pub/Sub (Publish/Subscribe) patterns using the high-performance **NATS.Net** cilent and NATS server.

This project acts as the foundational playground for understanding real-time, decoupled message routing before scaling up to distributed microservices.

---

## Architecture Overview

This demo uses 'Task.Run' to spin up a background worker (Subscriber) side-by-side with the main thread (Publisher). Both communicate instantly through a local NATS server running in Docker.

## Key Techical Concepts Covered:
* **In-Memory Routing:** Messages are routed purely via the NATS server's RAM and are delivered in microseconds.
* **JSON Serialization:** Uses 'NatsJsonSerializerRegistry' to automatically serialize and deserialize custom C# records ('StockTicker').
* **Asynchronous Coordination:** Leverages 'CancellationTokenSource' to gracefully shut down the perpetual background subscription loop when the application exists.

---

## Prerequisites

To run this demo, ensure you have the following installed:
* [.NET 8.0 or .NET 9.0 SDK] (https://dotnet.microsoft.com/download)
* [Docker Desktop] (https://www.docker.com/products/docker-desktop/)

---

## Getting Started

### 1. Spin up the NATS Server
Run the official, ultra-lightweight NATS image locally using Docker. It maps to the default NATS port '4222'.

'''bash
docker run -d --name nats-main -p 4222:4222 nats:latest

### 2. Run the Project
Navigate into the project directory and run the application:

'''bash
dotnet run

### 3. Experience the Flow
1. The terminal will initialize and show that the background subscriber is listening on the stocks.updates subject.
2. Press Any Key to trigger the publisher loop.
3. Watch the publisher fire 10 sequential stock ticks, which are instantly caught and logged by the background subscriber thread.
