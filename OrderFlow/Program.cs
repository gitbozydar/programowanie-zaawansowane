class Program
{
    static async Task Main(string[] args)
    {
        var orders = SampleData.Orders;

        Console.WriteLine($"Orders count: {orders.Count}");
        var processor = new OrderProcessor(orders);

        Predicate<Order> highValue = o => o.TotalPrice > 500;
        Predicate<Order> notCancelled = o => o.Status != OrderStatus.Cancelled;

        var filtered = processor.Filter(highValue);

        Console.WriteLine("=== FILTER ===");
        Console.WriteLine(filtered.Count);

        Action<Order> print = o =>
            Console.WriteLine($"{o.Customer.Username} | {o.TotalPrice} | {o.Status}");

        Console.WriteLine("\n=== ACTION ===");
        processor.ForEach(print);

        Func<Order, object> project = o => new
        {
            Customer = o.Customer.Username,
            Total = o.TotalPrice,
            Status = o.Status
        };

        Console.WriteLine("\n=== FUNC ===");
        foreach (var o in orders)
            Console.WriteLine(project(o));

        Func<IEnumerable<Order>, decimal> sum = o =>
            (decimal)o.Sum(x => x.TotalPrice);

        Func<IEnumerable<Order>, decimal> avg = o =>
            (decimal)o.Average(x => x.TotalPrice);

        Func<IEnumerable<Order>, decimal> max = o =>
            (decimal)o.Max(x => x.TotalPrice);

        Console.WriteLine("\n=== AGGREGATION ===");
        Console.WriteLine("SUM: " + processor.Aggregate(sum));
        Console.WriteLine("AVG: " + processor.Aggregate(avg));
        Console.WriteLine("MAX: " + processor.Aggregate(max));
        Console.WriteLine("\n=== PIPELINE ===");

        var top = orders
            .Where(o => o.TotalPrice > 100)
            .OrderByDescending(o => o.TotalPrice)
            .Take(3)
            .ToList();

        top.ForEach(o =>
            Console.WriteLine($"TOP: {o.TotalPrice} | {o.Status}")
        );

        Console.WriteLine("\n=== LINQ  ===");

        var join = SampleData.Orders
            .Join(SampleData.Customers,
                o => o.Customer.Id,
                c => c.Id,
                (o, c) => new { c.Username, o.TotalPrice });

        foreach (var x in join)
            Console.WriteLine(x.Username + " -> " + x.TotalPrice);

        var products = SampleData.Orders
            .SelectMany(o => o.Items)
            .Select(i => i.Product);

        foreach (var p in products)
            Console.WriteLine("Product: " + p.Category);

        var groups = SampleData.Orders
            .GroupBy(o => o.Customer.Username)
            .Select(g => new
            {
                Name = g.Key,
                Total = g.Sum(x => x.TotalPrice)
            });

        foreach (var g in groups)
            Console.WriteLine(g.Name + " -> " + g.Total);

        var leftJoin = SampleData.Customers
            .GroupJoin(SampleData.Orders,
                c => c.Id,
                o => o.Customer.Id,
                (c, orders) => new
                {
                    c.Username,
                    Count = orders.Count()
                });

        foreach (var x in leftJoin)
            Console.WriteLine(x.Username + " orders: " + x.Count);

        var query =
            from o in SampleData.Orders
            join c in SampleData.Customers
            on o.Customer.Id equals c.Id
            select new { c.Username, o.TotalPrice };

        foreach (var x in query)
            Console.WriteLine(x.Username + " -> " + x.TotalPrice);

        var pipeline = new OrderPipeline();

        Console.WriteLine("lab2");

        pipeline.StatusChanged += (sender, e) =>
        {
            Console.WriteLine($"[Logger] Order status changed: {e.OldStatus} → {e.NewStatus}");
        };

        pipeline.StatusChanged += (sender, e) =>
        {
            Console.WriteLine($"[Email] Sending email: Order for {e.Order.Customer.Username} is now {e.NewStatus}");
        };

        pipeline.StatusChanged += (sender, e) =>
        {
            Console.WriteLine($"[Stats] Increment counter for status {e.NewStatus}");
        };

        var customer = new Customer(1, "jakub_kozlowski", "jakubkozlo@wp.pl");
        var order = new Order(customer);
        pipeline.ProccessOrder(order);

        var allOrders = SampleData.Orders;

        var simulator = new ExternalServiceSimulator();

        Console.WriteLine("=== SINGLE ORDER ===");
        await simulator.ProcessOrderAsync(allOrders[0]);

        Console.WriteLine("\n=== MULTIPLE ORDERS ===");
        await simulator.ProcessMultipleOrdersAsync(allOrders);

        var repo = new OrderRepository();
        var testOrders = SampleData.Orders;

        await repo.SaveToJsonAsync(testOrders, "Data/orders.json");
        await repo.SaveToXmlAsync(testOrders, "Data/orders.xml");

        var json = await repo.LoadFromJsonAsync("Data/orders.json");
        var xml = await repo.LoadFromXmlAsync("Data/orders.xml");

        Console.WriteLine(json.Count);
        Console.WriteLine(xml.Count);
        Console.WriteLine(json.Sum(o => o.TotalPrice));
        Console.WriteLine(xml.Sum(o => o.TotalPrice));
    }
}