using InstagramClone.Web.Models;
using Microsoft.AspNetCore.Components;

namespace InstagramClone.Web.Services;

public class AuthService
{
    private readonly ApiService _apiService;
    private readonly LocalStorageService _localStorage;
    private readonly NavigationManager _navigation;
    private readonly TokenAuthenticationStateProvider _authStateProvider;
    private readonly ILogger<AuthService> _logger;

    public event Action? OnAuthStateChanged;

    public AuthService(ApiService apiService, LocalStorageService localStorage,
        NavigationManager navigation, TokenAuthenticationStateProvider authStateProvider, ILogger<AuthService> logger)
    {
        _apiService = apiService;
        _localStorage = localStorage;
        _navigation = navigation;
        _authStateProvider = authStateProvider;
        _logger = logger;
    }

    public async Task<bool> RegisterAsync(string email, string username, string password)
    {
        var response = await _apiService.RegisterAsync(email, username, password);

        if (response?.Success == true && response.Token != null)
        {
            await _localStorage.SetItemAsync("token", response.Token);
            await _localStorage.SetItemAsync("user", System.Text.Json.JsonSerializer.Serialize(response.User));
            _authStateProvider.NotifyUserAuthentication();
            OnAuthStateChanged?.Invoke();
            _logger.LogInformation($"User registered: {email}");
            return true;
        }

        return false;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _apiService.LoginAsync(email, password);

        if (response?.Success == true && response.Token != null)
        {
            await _localStorage.SetItemAsync("token", response.Token);
            await _localStorage.SetItemAsync("user", System.Text.Json.JsonSerializer.Serialize(response.User));
            _authStateProvider.NotifyUserAuthentication();
            OnAuthStateChanged?.Invoke();
            _logger.LogInformation($"User logged in: {email}");
            _navigation.NavigateTo("/dashboard");
            return true;
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("token");
        await _localStorage.RemoveItemAsync("user");
        _authStateProvider.NotifyUserLogout();
        OnAuthStateChanged?.Invoke();
        _navigation.NavigateTo("/");
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsync("token");
        return !string.IsNullOrEmpty(token);
    }

    public async Task<UserData?> GetCurrentUserAsync()
    {
        var userJson = await _localStorage.GetItemAsync("user");
        if (string.IsNullOrEmpty(userJson))
            return null;

        return System.Text.Json.JsonSerializer.Deserialize<UserData>(userJson);
    }
}
