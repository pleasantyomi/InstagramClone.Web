using System.Net.Http.Headers;
using System.Net.Http.Json;
using InstagramClone.Web.Models;
using Microsoft.AspNetCore.Components;

namespace InstagramClone.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly LocalStorageService _localStorage;
    private readonly ILogger<ApiService> _logger;
    private readonly string _baseUrl;

    public ApiService(HttpClient http, LocalStorageService localStorage, ILogger<ApiService> logger, IConfiguration configuration, NavigationManager navigationManager)
    {
        _http = http;
        _localStorage = localStorage;
        _logger = logger;

        if (Uri.TryCreate(navigationManager.BaseUri, UriKind.Absolute, out var currentUri) &&
            (string.Equals(currentUri.Host, "localhost", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(currentUri.Host, "127.0.0.1", StringComparison.OrdinalIgnoreCase)))
        {
            _baseUrl = "http://localhost:5167"; // Use local API endpoint for development
        }
        else
        {
            _baseUrl = (configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl is not configured."))
                .TrimEnd('/');
        }
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _localStorage.GetItemAsync("token");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    // ===== AUTH ENDPOINTS =====
    public virtual async Task<AuthResponse?> RegisterAsync(string email, string username, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"{_baseUrl}/api/auth/register", new
            {
                email,
                username,
                password
            });

            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Registration error: {ex.Message}");
            return null;
        }
    }

    public virtual async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"{_baseUrl}/api/auth/login", new
            {
                email,
                password
            });

            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login error: {ex.Message}");
            return null;
        }
    }

    public virtual async Task<bool> ValidateTokenAsync()
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _http.GetAsync($"{_baseUrl}/api/auth/validate");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // ===== POST ENDPOINTS =====
    public virtual async Task<List<PostModel>> GetFeedAsync(int page = 1)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _http.GetAsync($"{_baseUrl}/api/posts/feed?page={page}");
            return await response.Content.ReadFromJsonAsync<List<PostModel>>() ?? new List<PostModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get feed error: {ex.Message}");
            return new List<PostModel>();
        }
    }

    public virtual async Task<List<PostModel>> SearchPostsAsync(string query, string? location = null, string? person = null, int page = 1)
    {
        try
        {
            await SetAuthHeaderAsync();
            var url = $"{_baseUrl}/api/posts/search?query={Uri.EscapeDataString(query)}&page={page}";

            if (!string.IsNullOrWhiteSpace(location))
            {
                url += $"&location={Uri.EscapeDataString(location)}";
            }

            if (!string.IsNullOrWhiteSpace(person))
            {
                url += $"&person={Uri.EscapeDataString(person)}";
            }

            var response = await _http.GetAsync(url);
            return await response.Content.ReadFromJsonAsync<List<PostModel>>() ?? new List<PostModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Search posts error: {ex.Message}");
            return new List<PostModel>();
        }
    }

    public virtual async Task<bool> CreatePostAsync(CreatePostModel model)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _http.PostAsJsonAsync($"{_baseUrl}/api/posts", new
            {
                title = model.Title,
                caption = model.Caption,
                location = model.Location,
                peoplePresent = model.PeoplePresent,
                imageUrl = model.ImageUrl
            });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Create post error: {ex.Message}");
            return false;
        }
    }

    public virtual async Task<List<CommentModel>> GetCommentsAsync(Guid postId)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _http.GetAsync($"{_baseUrl}/api/posts/{postId}/comments");
            return await response.Content.ReadFromJsonAsync<List<CommentModel>>() ?? new List<CommentModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get comments error: {ex.Message}");
            return new List<CommentModel>();
        }
    }

    public virtual async Task<CommentModel?> AddCommentAsync(Guid postId, string content)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _http.PostAsJsonAsync($"{_baseUrl}/api/posts/{postId}/comments", new
            {
                content
            });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CommentModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Add comment error: {ex.Message}");
            return null;
        }
    }

    public virtual async Task<PostRatingResponse?> RatePostAsync(Guid postId, int rating)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _http.PostAsJsonAsync($"{_baseUrl}/api/posts/{postId}/ratings", new
            {
                rating
            });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<PostRatingResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Rate post error: {ex.Message}");
            return null;
        }
    }

    // ===== USER PROFILE ENDPOINTS =====
    public virtual async Task<UserProfile?> GetUserProfileAsync(Guid userId)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _http.GetAsync($"{_baseUrl}/api/users/{userId}/profile");
            return await response.Content.ReadFromJsonAsync<UserProfile>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get profile error: {ex.Message}");
            return null;
        }
    }

    public virtual async Task<List<UserProfile>> GetFollowersAsync(Guid userId)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _http.GetAsync($"{_baseUrl}/api/users/{userId}/followers");
            return await response.Content.ReadFromJsonAsync<List<UserProfile>>() ?? new List<UserProfile>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get followers error: {ex.Message}");
            return new List<UserProfile>();
        }
    }
}