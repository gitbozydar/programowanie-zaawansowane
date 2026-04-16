using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ExternalServiceSimulator
{
    private static readonly Random _random = new();

    public async Task<bool> CheckInventoryAsync(Product product)
    {
        await Task.Delay(_random.Next(500, 1500));
        return true;
    }

    public async Task<bool> ValidatePaymentAsync(Order order)
    {
        await Task.Delay(_random.Next(1000, 2000));
        return true;
    }

    public async Task<bool> CalculateShippingAsync(Order order)
    {
        await Task.Delay(_random.Next(300, 800));
        return true;
    }

    public async Task ProcessOrderAsync(Order order)
    {
        var stopwatch = Stopwatch.StartNew();

        var inventoryTask = CheckInventoryAsync(order.Items.First().Product);
        var paymentTask = ValidatePaymentAsync(order);
        var shippingTask = CalculateShippingAsync(order);

        await Task.WhenAll(inventoryTask, paymentTask, shippingTask);

        stopwatch.Stop();

        Console.WriteLine($"Czas wykonania: {stopwatch.ElapsedMilliseconds} ms");
    }

    public async Task ProcessMultipleOrdersAsync(List<Order> orders)
    {
        using var semaphore = new SemaphoreSlim(3);
        int processed = 0;

        var tasks = orders.Select(async order =>
        {
            await semaphore.WaitAsync();

            try
            {
                await ProcessOrderAsync(order);
            }
            finally
            {
                int done = Interlocked.Increment(ref processed);
                Console.WriteLine($"Przetworzono {done}/{orders.Count} zamówień");
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }
}