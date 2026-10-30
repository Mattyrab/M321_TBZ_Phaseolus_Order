namespace OrderBeans.Models;

public record OrderItem(string Name, int Quantity);

public record CreateOrderRequest(List<OrderItem> Items, decimal TotalAmount, string Email, string? DeliveryNote = null);

// DeliveryNote is new and optional -- an additive change existing consumers don't
// need to know about (message-contracts-hardening plan, Tolerant Reader demo).
public record OrderCreatedPayload(List<OrderItem> Items, decimal TotalAmount, string Email, string? DeliveryNote = null);
