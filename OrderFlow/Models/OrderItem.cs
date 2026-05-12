public class OrderItem
{
    public int Id { get; set; }

    public Product Product { get; set; }
    public int ProductId { get; set; }
    public int OrderId { get; set; }

    public int Quantity { get; set; }

    public double UnitPrice { get; set; }

    public double TotalPrice => UnitPrice * Quantity;

    public OrderItem() { }

    public OrderItem(Product product, int quantity)
    {
        Product = product;
        ProductId = product.Id;
        Quantity = quantity;
        UnitPrice = product.Price;
    }
}