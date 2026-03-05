using System.Collections.Concurrent;
using EventFlowInspector.Domain.Models;

namespace EventFlowInspector.Application;

public sealed class MessageTraceService : IMessageTraceService
{
    private readonly ConcurrentDictionary<string, List<MessageTrace>> _tracesByPlate = new(StringComparer.OrdinalIgnoreCase);

    public void AddTrace(MessageTrace trace)
    {
        var list = _tracesByPlate.GetOrAdd(trace.Plate, _ => new List<MessageTrace>());
        lock (list)
        {
            list.Add(trace);
        }
    }

    public IReadOnlyList<MessageTrace> GetByPlate(string plate)
    {
        if (!_tracesByPlate.TryGetValue(plate, out var traces))
        {
            return [];
        }

        lock (traces)
        {
            return traces.OrderBy(t => t.Timestamp).ToList();
        }
    }

    public IReadOnlyList<MessageTrace> GetByEventId(string eventId)
    {
        return _tracesByPlate
            .SelectMany(pair =>
            {
                lock (pair.Value)
                {
                    return pair.Value.ToArray();
                }
            })
            .Where(t => string.Equals(t.EventId, eventId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(t => t.Timestamp)
            .ToList();
    }

    public IReadOnlyList<MessageTrace> GetAll()
    {
        return _tracesByPlate
            .SelectMany(pair =>
            {
                lock (pair.Value)
                {
                    return pair.Value.ToArray();
                }
            })
            .OrderBy(t => t.Timestamp)
            .ToList();
    }
}
