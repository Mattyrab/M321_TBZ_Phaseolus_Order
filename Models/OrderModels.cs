namespace OrderBeans.Models;

public record OrderItem(string Name, int Quantity);

public record CreateOrderRequest(List<OrderItem> Items, decimal TotalAmount);

public record OrderCreatedPayload(List<OrderItem> Items, decimal TotalAmount);
