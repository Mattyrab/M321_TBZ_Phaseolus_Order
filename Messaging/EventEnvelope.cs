using System.Text.Json;

namespace OrderBeans.Messaging;

public record EventEnvelope(
    Guid EventId,
    string EventType,
    DateTimeOffset OccurredAt,
    Guid OrderId,
    JsonElement Payload);
