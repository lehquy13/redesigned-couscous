using EventFlowInspector.Application;

namespace EventFlowInspector.Api;

public static class SearchEndpoints
{
    public static RouteGroupBuilder MapSearchEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/search", (
            string queue,
            DateTime? startTime,
            DateTime? endTime,
            int? partition,
            long? offsetStart,
            long? offsetEnd,
            IMessageSearchService searchService) =>
        {
            var results = searchService.Search(queue, startTime, endTime, partition, offsetStart, offsetEnd);
            return Results.Ok(results);
        });

        return group;
    }
}
