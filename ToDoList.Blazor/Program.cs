using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using ToDoList.Blazor.Components;
using Services;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi necessari per Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configura HttpClient per comunicare con l'API
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7152/");
});

builder.Services.AddHttpClient<ToDoService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7152/");
});

var app = builder.Build();

// Configura la pipeline di richiesta HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();