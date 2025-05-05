using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using ToDoList.Blazor.Components;
using Services;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi necessari per Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registra HttpClient globale
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7152/") });

// Registra AuthService e ToDoService come servizi scoped
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ToDoService>();

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
