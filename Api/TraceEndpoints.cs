using EventFlowInspector.Application;
using EventFlowInspector.Domain.Models;

namespace EventFlowInspector.Api;

public static class TraceEndpoints
{
    public static RouteGroupBuilder MapTraceEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/trace", (string? plate, string? eventId, IMessageTraceService traceService) =>
        {
            if (string.IsNullOrWhiteSpace(plate) && string.IsNullOrWhiteSpace(eventId))
            {
                return Results.BadRequest("Provide either plate or eventId query parameter.");
            }

            var traces = !string.IsNullOrWhiteSpace(plate)
                ? traceService.GetByPlate(plate)
                : traceService.GetByEventId(eventId!);

            var queueFlow = Enum.GetNames<KafkaQueue>()
                .Select(q => new
                {
                    queue = q,
                    seen = traces.Any(t => string.Equals(t.Queue, q, StringComparison.OrdinalIgnoreCase))
                });

            return Results.Ok(new
            {
                plate,
                eventId,
                traces,
                queueFlow
            });
        });

        return group;
    }
}
