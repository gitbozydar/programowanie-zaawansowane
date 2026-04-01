public class OrderProcessor
{
    private List<Order> _orders;

    public OrderProcessor(List<Order> orders)
    {
        _orders = orders;
    }

    public List<Order> Filter(Predicate<Order> predicate)
    {
        return _orders.Where(o => predicate(o)).ToList();
    }

    public void ForEach(Action<Order> action)
    {
        foreach (var order in _orders)
            action(order);
    }

    public decimal Aggregate(Func<IEnumerable<Order>, decimal> aggregator)
    {
        return aggregator(_orders);
    }
}