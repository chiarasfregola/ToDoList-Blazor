using Microsoft.AspNetCore.Components;
using Services;
using ToDoList.Blazor.Components;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Configura i servizi per Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//HttpClient con l'indirizzo del backend
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7152/")
});


builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ToDoService>();

var app = builder.Build();

//configura la pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

//mappa le componenti Razor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
