# Task

Create a .NET 8 Web API project called **EventFlow Inspector**.

This project is a lightweight debugging tool used to inspect and test message flows across a Kafka/EventHub event-driven system.

The tool must allow testers to:

- Generate sample messages
- Publish messages to Kafka/EventHub queues
- Track message flow across queues
- Search messages by time or offset
- Inspect pipeline health

The project must be designed for **local testing environments** and must be easy to migrate to another machine later.

---

# Technology Stack

Use:

- .NET 8
- Minimal API
- Kafka client (Confluent.Kafka)
- Bogus (for fake data generation)
- BackgroundService for consumers
- Native AOT compatible design

Avoid reflection-heavy libraries.

---

# Project Structure

Generate the following folder structure:

EventFlowInspector

Api  
- SampleEndpoints.cs  
- PublishEndpoints.cs  
- TraceEndpoints.cs  
- SearchEndpoints.cs  

Application  
- MessageGeneratorService.cs  
- MessagePublisherService.cs  
- MessageTraceService.cs  
- MessageSearchService.cs  

Infrastructure  

Messaging  
- KafkaProducer.cs  
- KafkaConsumer.cs  

Worker  
- EventListenerWorker.cs  

Domain  

Models  
- PlateEvent.cs  
- MessageTrace.cs  

Program.cs  
appsettings.json  

---

# Kafka Queue Definitions

Create an enum:

```csharp
public enum KafkaQueue
{
    RawInput,
    AIAnalyzer,
    Commercial,
    PublicSafety
}