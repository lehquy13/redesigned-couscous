using Bogus;
using EventFlowInspector.Domain.Models;

namespace EventFlowInspector.Application;

public sealed class MessageGeneratorService : IMessageGeneratorService
{
    public IReadOnlyList<PlateEvent> Generate(IEnumerable<string> inputs)
    {
        var faker = new Faker();
        var generated = new List<PlateEvent>();

        foreach (var input in inputs)
        {
            var parts = input.Split(':', 2, StringSplitOptions.TrimEntries);
            var plate = parts[0];
            var eventId = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1])
                ? parts[1]
                : Guid.NewGuid().ToString();

            generated.Add(new PlateEvent
            {
                EventId = eventId,
                Plate = plate,
                CameraId = faker.PickRandom("CAM01", "CAM02", "CAM03", "CAM04"),
                Timestamp = DateTime.UtcNow,
                VehicleType = faker.Vehicle.Type(),
                Color = faker.Commerce.Color()
            });
        }

        return generated;
    }
}
