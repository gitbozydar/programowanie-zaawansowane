public class OrderPipeline
{
    public event EventHandler<OrderStatusChangedEventArgs> StatusChanged;
    public event EventHandler<OrderValidationEventArgs> ValidationCompleted;



    public void ProccessOrder(Order order)
    {
        var oldStatus = order.Status;
        order.Status = OrderStatus.Validated;
        StatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, oldStatus, order.Status));

        oldStatus = order.Status;
        order.Status = OrderStatus.Processing;
        StatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, oldStatus, order.Status));

        oldStatus = order.Status;
        order.Status = OrderStatus.Completed;
        StatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, oldStatus, order.Status));

    }
}


public class OrderStatusChangedEventArgs : EventArgs
{
    public Order Order { get; }
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }

    public OrderStatusChangedEventArgs(Order order, OrderStatus oldStatus, OrderStatus newStatus)
    {
        Order = order;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}
public class OrderValidationEventArgs : EventArgs
{
    public Order Order { get; }
    public bool IsValid { get; }
    public List<string> Errors { get; }

    public OrderValidationEventArgs(Order order, bool isValid, List<string> errors)
    {
        Order = order;
        IsValid = isValid;
        Errors = errors ?? new List<string>();
    }
}