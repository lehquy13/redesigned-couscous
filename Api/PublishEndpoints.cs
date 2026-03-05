using EventFlowInspector.Application;
using EventFlowInspector.Domain.Models;

namespace EventFlowInspector.Api;

public static class PublishEndpoints
{
    public static RouteGroupBuilder MapPublishEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/publish/{queue}", async (KafkaQueue queue, string body, int? partition, IMessagePublisherService publisher, CancellationToken ct) =>
        {
            try
            {
                var result = await publisher.PublishAsync(queue, body, partition, ct);
                return Results.Ok(result);
            }
            catch (TimeoutException ex)
            {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        });

        return group;
    }
}
