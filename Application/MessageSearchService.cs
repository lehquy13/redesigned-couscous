using EventFlowInspector.Domain.Models;

namespace EventFlowInspector.Application;

public sealed class MessageSearchService(IMessageTraceService traceService) : IMessageSearchService
{
    public IReadOnlyList<MessageTrace> Search(string queue, DateTime? startTime, DateTime? endTime, int? partition, long? offsetStart, long? offsetEnd)
    {
        var query = traceService
            .GetAll()
            .Where(t => string.Equals(t.Queue, queue, StringComparison.OrdinalIgnoreCase));

        if (startTime.HasValue)
        {
            query = query.Where(t => t.Timestamp >= startTime.Value);
        }

        if (endTime.HasValue)
        {
            query = query.Where(t => t.Timestamp <= endTime.Value);
        }

        if (partition.HasValue)
        {
            query = query.Where(t => t.Partition == partition.Value);
        }

        if (offsetStart.HasValue)
        {
            query = query.Where(t => t.Offset >= offsetStart.Value);
        }

        if (offsetEnd.HasValue)
        {
            query = query.Where(t => t.Offset <= offsetEnd.Value);
        }

        return query.OrderBy(t => t.Timestamp).ToList();
    }
}
