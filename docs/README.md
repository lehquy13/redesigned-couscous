# EventFlow Inspector Diagrams

This folder contains PlantUML diagrams for architecture and runtime flow.

## Files
- `architecture-diagram.puml`: high-level layered architecture and dependencies.
- `publish-trace-sequence.puml`: sequence from publish request through Kafka and trace retrieval.

## Render
Use any PlantUML renderer, for example:

```bash
plantuml docs/architecture-diagram.puml docs/publish-trace-sequence.puml
```
