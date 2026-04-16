public class Order
{
    public Customer Customer { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; private set; } = new List<OrderItem>();
    public double TotalPrice => Items.Sum(item => item.TotalPrice);
    public Order(Customer customer, OrderStatus status = OrderStatus.New)
    {
        Customer = customer;
        Status = status;
    }

    public void AddItem(Product product, int quantity)
    {
        Items.Add(new OrderItem(product, quantity));

    }
    public Order()
    {
    }

}