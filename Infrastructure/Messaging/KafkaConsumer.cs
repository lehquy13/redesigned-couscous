using System.Text.Json;
using Confluent.Kafka;
using EventFlowInspector.Application;
using EventFlowInspector.Domain.Models;
using Microsoft.Extensions.Options;

namespace EventFlowInspector.Infrastructure.Messaging;

public sealed class KafkaConsumer(IOptions<KafkaOptions> kafkaOptions)
{
    public IConsumer<string, string> Build(string consumerGroup)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.BootstrapServers,
            GroupId = consumerGroup,
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = true
        };

        return new ConsumerBuilder<string, string>(config).Build();
    }

    public static MessageTrace ToTrace(string queue, ConsumeResult<string, string> consumed)
    {
        var plateEvent = JsonSerializer.Deserialize<PlateEvent>(consumed.Message.Value);

        return new MessageTrace
        {
            Plate = plateEvent?.Plate ?? consumed.Message.Key,
            EventId = plateEvent?.EventId,
            Queue = queue,
            Partition = consumed.Partition.Value,
            Offset = consumed.Offset.Value,
            Timestamp = consumed.Message.Timestamp.UtcDateTime,
            RawMessage = consumed.Message.Value
        };
    }
}
