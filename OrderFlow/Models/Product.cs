public class Product
{
    public double Price { get; set; }
    public int Stock { get; set; }
    public int Id { get; set; }

    public string Category { get; set; }
    public Product(double price, int stock, string category)
    {
        Price = price;
        Stock = stock;
        Category = category;
    }
    public Product()
    {
    }
}