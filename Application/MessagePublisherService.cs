using EventFlowInspector.Domain.Models;
using EventFlowInspector.Infrastructure.Messaging;

namespace EventFlowInspector.Application;

public sealed class MessagePublisherService(
    KafkaProducer producer,
    IMessageTraceService traceService,
    IListenerReadiness listenerReadiness) : IMessagePublisherService
{
    public async Task<object> PublishAsync(KafkaQueue queue, string rawMessage, int? partition, CancellationToken cancellationToken)
    {
        await listenerReadiness.WaitUntilReadyAsync(cancellationToken);

        if (partition.HasValue)
        {
            await listenerReadiness.EnsurePartitionListenerAsync(queue, partition.Value, cancellationToken);
        }

        var trace = await producer.PublishAsync(queue, rawMessage, partition, cancellationToken);
        traceService.AddTrace(trace);

        return new
        {
            queue = trace.Queue,
            partition = trace.Partition,
            offset = trace.Offset,
            timestamp = trace.Timestamp
        };
    }
}
