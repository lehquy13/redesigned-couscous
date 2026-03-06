using System.Text.Json;
using Confluent.Kafka;
using EventFlowInspector.Application;
using EventFlowInspector.Domain.Models;
using Microsoft.Extensions.Options;

namespace EventFlowInspector.Infrastructure.Messaging;

public sealed class KafkaProducer : IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _kafkaOptions;

    public KafkaProducer(IOptions<KafkaOptions> kafkaOptions)
    {
        _kafkaOptions = kafkaOptions.Value;
        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            Acks = Acks.All
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task<MessageTrace> PublishAsync(KafkaQueue queue, string payload, int? partition, CancellationToken cancellationToken)
    {
        var plateEvent = JsonSerializer.Deserialize<PlateEvent>(payload);
        var message = new Message<string, string>
        {
            Key = plateEvent?.Plate ?? Guid.NewGuid().ToString("N"),
            Value = payload,
            Timestamp = new Timestamp(DateTime.UtcNow)
        };

        DeliveryResult<string, string> result;
        var topic = _kafkaOptions.ResolveTopic(queue);

        if (partition.HasValue)
        {
            result = await _producer.ProduceAsync(new TopicPartition(topic, new Partition(partition.Value)), message, cancellationToken);
        }
        else
        {
            result = await _producer.ProduceAsync(topic, message, cancellationToken);
        }

        return new MessageTrace
        {
            Plate = plateEvent?.Plate ?? string.Empty,
            EventId = plateEvent?.EventId,
            Queue = queue.ToString(),
            Partition = result.Partition.Value,
            Offset = result.Offset.Value,
            Timestamp = result.Timestamp.UtcDateTime,
            RawMessage = payload
        };
    }

    public void Dispose() => _producer.Dispose();
}
