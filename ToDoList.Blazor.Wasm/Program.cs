using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ToDoList.Blazor.Wasm;
using ToDoList.Blazor.Wasm.Services;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication; // per AddOidcAuthentication

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//configura HttpClient per comunicare con il backend API
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7152/") //URL del backend API
    };

    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    return httpClient;
});

//aggiunge il supporto per LocalStorage
builder.Services.AddBlazoredLocalStorage();

//aegistrazione dei servizi personalizzati
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ToDoService>();


builder.Services.AddOidcAuthentication(options =>
{
    // Il nome "Local" deve corrispondere a una sezione nel file appsettings.json
    builder.Configuration.Bind("Local", options.ProviderOptions);
});

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();



