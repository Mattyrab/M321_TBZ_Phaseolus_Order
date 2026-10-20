using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OrderBeans.HealthChecks;
using OrderBeans.Messaging;
using OrderBeans.Models;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddSingleton<EventPublisher>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.RequireHttpsMetadata = false; // Keycloak runs on plain HTTP inside the cluster (no TLS hardening, per the architecture plan's non-goals)
        options.TokenValidationParameters.ValidateAudience = false;
    });
builder.Services.AddAuthorization();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: ["ready"]);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpMetrics();
app.MapMetrics();

app.MapHealthChecks("/healthz/live", new() { Predicate = c => c.Tags.Contains("live") });
app.MapHealthChecks("/healthz/ready", new() { Predicate = c => c.Tags.Contains("ready") });

app.MapPost("/orders", (CreateOrderRequest request, EventPublisher publisher) =>
{
    var orderId = Guid.NewGuid();
    var payload = new OrderCreatedPayload(request.Items, request.TotalAmount);

    var envelope = new EventEnvelope(
        EventId: Guid.NewGuid(),
        EventType: "order.created",
        OccurredAt: DateTimeOffset.UtcNow,
        OrderId: orderId,
        Payload: JsonSerializer.SerializeToElement(payload));

    publisher.Publish("order.created", envelope);

    return Results.Accepted(value: new { orderId });
}).RequireAuthorization();

app.Run();
