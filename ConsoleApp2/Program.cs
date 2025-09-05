using Newtonsoft.Json;

namespace ConsoleApp2;

public class Program
{
    public static async Task Main()
    {
        Processor p = new Processor();
        await p.Process();
    }
}
public class Processor
{
    private readonly HttpClient _client = new() { BaseAddress = new Uri("http://data.com/") };

    public async Task Process()
    {
        // Get sales orders list.
        var salesOrders = await GetAsync<List<SalesOrder>>("api/salesorders?minDate=2023-01-01");
        if (salesOrders == null || salesOrders.Count == 0)
        {
            Console.WriteLine("No sales orders found or request failed.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }

        // Get customer Ids from sales orders
        var customerIds = salesOrders.Select(s => s.CustomerId).Distinct().ToList();
        // Get customers list.
        var customers = await GetAsync<List<Customer>>($"api/customers?customerIds={string.Join(",", customerIds)}");
        if (customers == null || customers.Count == 0)
        {
            Console.WriteLine(@"No customers found.");
            Console.WriteLine(@"Press any key to exit...");
            Console.ReadKey();
            return;
        }

        // Create dictionary of customers and their sales orders
        var dictionary = customers.ToDictionary(
            customer => customer,
            customer => salesOrders.Where(s => s.CustomerId == customer.CustomerId).ToList()
        );

        // Save to JSON file
        await File.WriteAllTextAsync(@"c:\interview.json",
            JsonConvert.SerializeObject(dictionary, Formatting.Indented));

        Console.WriteLine(@"Data saved to c:\\interview.json");
        Console.WriteLine(@"Press any key to exit...");
        Console.ReadKey();
    }

    // Helper method to perform GET requests and deserialize the response using a DRY pattern (Don't Repeat Yourself). 
    // This could go into a HTTP Service along with the rest of the OAuth spec for reuse everywhere.
    private async Task<T?> GetAsync<T>(string url) where T : class
    {
        var response = await _client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Request failed: {response.ReasonPhrase}");
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(content);
    }
}

// Data Transfer Objects (DTOs)
public class Customer
{
    public int CustomerId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class SalesOrder
{
    public int SalesOrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTimeOffset OrderDateTime { get; set; }
    public List<LineItem> LineItems { get; set; } = new();
}

public class LineItem
{
    public int LineItemId { get; set; }
    public int SalesOrderId { get; set; }
    public string? ItemName { get; set; }
    public decimal ItemPrice { get; set; }
}