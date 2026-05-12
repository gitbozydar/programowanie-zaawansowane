using System.Globalization;
using System.Xml.Linq;

public class XmlReportBuilder
{
    public XDocument BuildReport(IEnumerable<Order> orders)
    {
        var orderList = orders.ToList();

        var report = new XDocument(
            new XElement("report",
                new XAttribute("generated", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),

                // SUMMARY
                new XElement("summary",
                    new XAttribute("totalOrders", orderList.Count),
                    new XAttribute(
                        "totalRevenue",
                        orderList.Sum(o => o.TotalPrice)
                                 .ToString("F2", CultureInfo.InvariantCulture)
                    )
                ),
                new XElement("byStatus",
                    orderList
                        .GroupBy(o => o.Status)
                        .Select(g =>
                            new XElement("status",
                                new XAttribute("name", g.Key.ToString()),
                                new XAttribute("count", g.Count()),
                                new XAttribute(
                                    "revenue",
                                    g.Sum(o => o.TotalPrice)
                                     .ToString("F2", CultureInfo.InvariantCulture)
                                )
                            )
                        )
                ),
                new XElement("byCustomer",
                    orderList
                        .GroupBy(o => o.Customer)
                        .Select(g =>
                            new XElement("customer",
                                new XAttribute("id", g.Key.Id),
                                new XAttribute("name", g.Key.Username),

                                new XElement("orderCount", g.Count()),

                                new XElement(
                                    "totalSpent",
                                    g.Sum(o => o.TotalPrice)
                                     .ToString("F2", CultureInfo.InvariantCulture)
                                ),

                                new XElement("orders",
                                    g.Select(o =>
                                        new XElement("orderRef",
                                            new XAttribute("id", o.Id),
                                            new XAttribute(
                                                "total",
                                                o.TotalPrice
                                                 .ToString("F2", CultureInfo.InvariantCulture)
                                            )
                                        )
                                    )
                                )
                            )
                        )
                )
            )
        );

        return report;
    }

    public async Task SaveReportAsync(XDocument report, string path)
    {
        var directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await Task.Run(() => report.Save(path));
    }

    public async Task<IEnumerable<int>> FindHighValueOrderIdsAsync(
        string reportPath,
        decimal threshold)
    {
        var document = await Task.Run(() => XDocument.Load(reportPath));

        var ids = document
            .Descendants("orderRef")
            .Where(x =>
                decimal.Parse(
                    x.Attribute("total")!.Value,
                    CultureInfo.InvariantCulture
                ) > threshold
            )
            .Select(x =>
                int.Parse(x.Attribute("id")!.Value)
            )
            .ToList();

        return ids;
    }
}