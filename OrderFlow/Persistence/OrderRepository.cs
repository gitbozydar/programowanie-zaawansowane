using System.Text.Json;
using System.Xml.Serialization;

public class OrderRepository
{
    public async Task SaveToJsonAsync(IEnumerable<Order> orders, string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        await using var stream = File.Create(path);

        await JsonSerializer.SerializeAsync(stream, orders, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        Console.WriteLine($"PATH: {path}");
        Console.WriteLine($"DIR: {Path.GetDirectoryName(path)}");
    }
    public async Task<List<Order>> LoadFromJsonAsync(string path)
    {
        if (!File.Exists(path))
            return new List<Order>();

        await using var stream = File.OpenRead(path);

        return await JsonSerializer.DeserializeAsync<List<Order>>(stream)
               ?? new List<Order>();
    }
    public async Task SaveToXmlAsync(IEnumerable<Order> orders, string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var serializer = new XmlSerializer(typeof(List<Order>));

        await using var stream = File.Create(path);
        serializer.Serialize(stream, orders.ToList());
        Console.WriteLine($"PATH: {path}");
        Console.WriteLine($"DIR: {Path.GetDirectoryName(path)}");
        Console.WriteLine("CWD: " + Directory.GetCurrentDirectory());
    }
    public async Task<List<Order>> LoadFromXmlAsync(string path)
    {
        if (!File.Exists(path))
            return new List<Order>();

        var serializer = new XmlSerializer(typeof(List<Order>));

        await using var stream = File.OpenRead(path);

        return (List<Order>?)serializer.Deserialize(stream)
               ?? new List<Order>();
    }
}