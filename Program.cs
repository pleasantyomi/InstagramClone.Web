using InstagramClone.Web;
using InstagramClone.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<TokenAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<TokenAuthenticationStateProvider>());

// Toggle between real API and mock API
// SET USE_MOCK_API = false to use real backend, true to use demo data
#pragma warning disable CS0162
const bool USE_MOCK_API = false;

if (USE_MOCK_API)
{
    builder.Services.AddScoped<ApiService, MockApiService>();
}
else
{
    builder.Services.AddScoped<ApiService>();
}
#pragma warning restore CS0162

builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();
