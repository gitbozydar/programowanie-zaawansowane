public static class SampleData
{
    public static List<Product> Products = new List<Product>
    {
        new Product(1500, 10, "Electronics"),
        new Product(50, 25, "Books"),
        new Product(120, 5, "Clothing"),
        new Product(300, 8, "Sports"),
        new Product(80, 15, "Toys")
    };

    public static List<Customer> Customers = new List<Customer>
    {
        new Customer(1, "Janek", "janek@email.com"),
        new Customer(2, "Kasia", "kasia@email.com"),
        new Customer(3, "Tomek", "tomek@email.com"),
        new Customer(4, "VIP_Anna", "vip_anna@email.com") // VIP
    };

    public static List<Order> Orders = new List<Order>();

    static SampleData()
    {
        // Order 1
        var order1 = new Order(Customers[0], OrderStatus.New);
        order1.AddItem(Products[0], 1);
        order1.AddItem(Products[1], 2);
        Orders.Add(order1);

        // Order 2
        var order2 = new Order(Customers[1], OrderStatus.Processing);
        order2.AddItem(Products[2], 1);
        Orders.Add(order2);

        // Order 3
        var order3 = new Order(Customers[2], OrderStatus.Validated);
        order3.AddItem(Products[3], 2);
        order3.AddItem(Products[4], 1);
        Orders.Add(order3);

        // Order 4
        var order4 = new Order(Customers[3], OrderStatus.New);
        order4.AddItem(Products[0], 2);
        order4.AddItem(Products[4], 3);
        Orders.Add(order4);

        // Order 5
        var order5 = new Order(Customers[0], OrderStatus.Completed);
        order5.AddItem(Products[1], 1);
        order5.AddItem(Products[2], 2);
        Orders.Add(order5);

        // Order 6
        var order6 = new Order(Customers[1], OrderStatus.Cancelled);
        order6.AddItem(Products[3], 1);
        Orders.Add(order6);
    }
}