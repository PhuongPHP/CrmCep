using Blazored.LocalStorage;
using CrmCep.BlazorUI.Components;
using CrmCep.BlazorUI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Razor components with Interactive Server support
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register MudBlazor component services
builder.Services.AddMudServices();

// Register LocalStorage for JWT session management
builder.Services.AddBlazoredLocalStorage();

// Register authentication core and custom JWT state provider
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Register HTTP client for calling Web API
var webApiUrl = builder.Configuration["WebApiBaseUrl"] ?? "https://localhost:7178/";
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(webApiUrl)
});

// Register API Client
builder.Services.AddScoped<ICustomerApiClient, CustomerApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
