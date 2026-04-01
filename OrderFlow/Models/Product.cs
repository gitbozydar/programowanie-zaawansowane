public class Product
{
    public double Price {get; private set;}
    public int Stock {get; private set;}
    public string Category {get; private set;}
    public Product (double price, int stock, string category)
    {
        Price = price;
        Stock = stock;
        Category = category;
    }
}