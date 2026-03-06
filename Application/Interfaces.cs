using EventFlowInspector.Domain.Models;

namespace EventFlowInspector.Application;

public interface IMessageGeneratorService
{
    IReadOnlyList<PlateEvent> Generate(IEnumerable<string> inputs);
}

public interface IMessagePublisherService
{
    Task<object> PublishAsync(KafkaQueue queue, string rawMessage, int? partition, CancellationToken cancellationToken);
}

public interface IMessageTraceService
{
    void AddTrace(MessageTrace trace);
    IReadOnlyList<MessageTrace> GetByPlate(string plate);
    IReadOnlyList<MessageTrace> GetByEventId(string eventId);
    IReadOnlyList<MessageTrace> GetAll();
}

public interface IMessageSearchService
{
    IReadOnlyList<MessageTrace> Search(string queue, DateTime? startTime, DateTime? endTime, int? partition, long? offsetStart, long? offsetEnd);
}

public interface IListenerReadiness
{
    bool IsReady { get; }
    Task WaitUntilReadyAsync(CancellationToken cancellationToken);
    Task EnsurePartitionListenerAsync(KafkaQueue queue, int partition, CancellationToken cancellationToken);
}
