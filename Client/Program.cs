using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using InventoryHubApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// builder.Configuration.AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    Console.WriteLine("hello");
    var apiBaseAddress = builder.Configuration.GetSection("ApiSettings:BaseUrl").Value;
    Console.WriteLine(apiBaseAddress);
    if (string.IsNullOrEmpty(apiBaseAddress))
    {
        throw new ArgumentNullException(nameof(apiBaseAddress), "API base address is not defined.");
    }

    client.BaseAddress = new Uri(apiBaseAddress);
});

await builder.Build().RunAsync();
