namespace OrderBeans.Models;

public record OrderItem(string Name, int Quantity);

public record CreateOrderRequest(List<OrderItem> Items, decimal TotalAmount, string Email);

public record OrderCreatedPayload(List<OrderItem> Items, decimal TotalAmount, string Email);
