using EventFlowInspector.Domain.Models;

namespace EventFlowInspector.Application;

public sealed class KafkaOptions
{
    public const string SectionName = "Kafka";

    public string BootstrapServers { get; init; } = "localhost:9092";

    public string ConsumerGroupPrefix { get; init; } = "eventflow-inspector";

    public Dictionary<string, string> Topics { get; init; } = new(StringComparer.OrdinalIgnoreCase)
    {
        [KafkaQueue.RawInput.ToString()] = KafkaQueue.RawInput.ToString(),
        [KafkaQueue.AIAnalyzer.ToString()] = KafkaQueue.AIAnalyzer.ToString(),
        [KafkaQueue.Commercial.ToString()] = KafkaQueue.Commercial.ToString(),
        [KafkaQueue.PublicSafety.ToString()] = KafkaQueue.PublicSafety.ToString()
    };

    public string ResolveTopic(KafkaQueue queue)
        => ResolveTopic(queue.ToString());

    public string ResolveTopic(string queueName)
        => Topics.TryGetValue(queueName, out var topic) ? topic : queueName;
}
