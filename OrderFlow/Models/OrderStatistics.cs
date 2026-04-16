using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

public class OrderStatistics
{
    public int TotalProcessed;
    public decimal TotalRevenue;

    public ConcurrentDictionary<string, int> OrdersPerStatus = new();
    public List<string> ProcessingErrors = new();

    private readonly object _lock = new();

    public void AddOrder(Order order)
    {
        Interlocked.Increment(ref TotalProcessed);

        lock (_lock)
        {
            TotalRevenue += (decimal)order.TotalPrice;
        }

        OrdersPerStatus.AddOrUpdate(
            order.Status.ToString(),
            1,
            (_, count) => count + 1
        );
    }

    public void AddError(string error)
    {
        lock (_lock)
        {
            ProcessingErrors.Add(error);
        }
    }
}