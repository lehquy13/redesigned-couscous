using EventFlowInspector.Application;
using EventFlowInspector.Domain.Models;

namespace EventFlowInspector.Api;

public static class SampleEndpoints
{
    public static RouteGroupBuilder MapSampleEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/sample-message/{queue}", (KafkaQueue queue, IEnumerable<string> lines, IMessageGeneratorService generator) =>
        {
            var generated = generator.Generate(lines);
            return Results.Ok(generated);
        });

        return group;
    }
}
