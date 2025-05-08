using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ToDoList.Blazor.Wasm;
using ToDoList.Blazor.Wasm.Services;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configura HttpClient per comunicare con il backend API
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7152/") // URL del backend API
    };

    // Aggiunta opzionale: imposta l'intestazione accettata per JSON
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    return httpClient;
});

// Aggiunge il supporto per LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Registrazione dei servizi personalizzati
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ToDoService>();

await builder.Build().RunAsync();

