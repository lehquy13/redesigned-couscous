# EventFlow Inspector — Requirements & Design

These queues represent stages in an LPR event pipeline.

---

## Table of Contents
- Overview
- Queues
- Endpoints
  - Sample Message Generator
  - Publish Message
  - Trace
  - Search
- Listener Worker
- Listener Readiness
- Message Trace Storage
- Configuration
- Native AOT Support
- Swagger
- Expected Output
- Design Constraints
- Project Name

---

## Overview
Lightweight API to generate, publish, listen, and trace LPR events using Kafka. In-memory trace storage for rapid local testing.

## Queues
- RawInput
- AIAnalyzer
- Commercial
- PublicSafety

---

## Endpoints

### 1) Sample Message Generator
POST /sample-message/{queue}

Request body format:
- Array of strings: `Plate:eventId` (eventId may be empty)

Rules:
- If eventId is empty → generate a new GUID
- Generate other fake fields using Bogus
- Return generated JSON messages

Example request body:
```
[
  "51F12345:",
  "ABC987:2a1f..."
]
```

Example response:
```json
[
  {
    "eventId": "guid",
    "plate": "51F12345",
    "cameraId": "CAM01",
    "timestamp": "2026-03-05T10:00:00Z"
  }
]
```

### 2) Publish Message
POST /publish/{queue}?partition={optional}

- Body: JSON message
- Producer must return metadata: `partition`, `offset`, `timestamp`
- Store metadata for trace purposes

Example response:
```json
{
  "queue": "RawInput",
  "partition": 1,
  "offset": 1201,
  "timestamp": "2026-03-05T10:00:05Z"
}
```

---

## Listener Worker
Background service: `EventListenerWorker`

Responsibilities:
- Subscribe to all queues from configuration
- Start from latest offset
- Kafka config: `AutoOffsetReset = Latest` (we only want events published during the test session)
- Read messages and store traces in memory

---

## Listener Readiness
Before allowing publish operations:
- Start all queue listeners
- Wait until consumers are ready
- Allow publish operations only after readiness

Configuration:
- `ListenerReadyTimeoutMinutes` (configurable)
- If timeout exceeded → reject publish requests

---

## Message Trace Storage
In-memory storage:
- `ConcurrentDictionary<string, List<MessageTrace>>`  
  Keyed by plate number

MessageTrace model:
- Plate
- Queue
- Partition
- Offset
- Timestamp
- EventId (optional)

Trace endpoint:
- GET /trace?plate={plate}
- GET /trace?eventId={eventId}

Example response (summary):
```
Plate: 51F12345

Queue Flow
RawInput        ✔
AIAnalyzer      ✔
Commercial      ❌
PublicSafety    ❌
```

---

## Robust Message Search
GET /search

Query parameters:
- `queue` (required)
- `startTime` (optional)
- `endTime` (optional)
- `partition` (optional)
- `offsetStart` (optional)
- `offsetEnd` (optional)

Examples:
- Search by time:
  `/search?queue=RawInput&startTime=2026-03-05T10:00&endTime=2026-03-05T10:10`
- Search by offset:
  `/search?queue=RawInput&offsetStart=1000&offsetEnd=2000`

---

## Configuration (appsettings.json)
Example:
```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092"
  },
  "EventHubs": {
    "RawInput": "connection",
    "AIAnalyzer": "connection",
    "Commercial": "connection",
    "PublicSafety": "connection"
  },
  "ListenerReadyTimeoutMinutes": 2
}
```
