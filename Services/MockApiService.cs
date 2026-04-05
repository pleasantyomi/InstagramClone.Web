using InstagramClone.Web.Models;
using Microsoft.AspNetCore.Components;

namespace InstagramClone.Web.Services;

public class MockApiService : ApiService
{
    private readonly ILogger<MockApiService> _logger;
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid DemoUser2Id = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private static readonly Guid DemoUser3Id = Guid.Parse("00000000-0000-0000-0000-000000000003");

    public MockApiService(HttpClient http, LocalStorageService localStorage, ILogger<MockApiService> logger, IConfiguration configuration, NavigationManager navigationManager)
        : base(http, localStorage, logger, configuration, navigationManager)
    {
        _logger = logger;
    }

    // ===== AUTH ENDPOINTS (OVERRIDE) =====
    public override async Task<AuthResponse?> RegisterAsync(string email, string username, string password)
    {
        await Task.Delay(500); // Simulate network delay
        _logger.LogInformation($"[MOCK] Register: {email}");

        return new AuthResponse
        {
            Success = true,
            Message = "Registration successful",
            Token = "demo-token-" + Guid.NewGuid().ToString().Substring(0, 8),
            ExpiresIn = 3600,
            User = new UserData
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                Role = "Consumer"
            }
        };
    }

    public override async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        await Task.Delay(500); // Simulate network delay
        _logger.LogInformation($"[MOCK] Login: {email}");

        // Demo login - accept any email/password for testing
        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            Token = "demo-token-abc123def456",
            ExpiresIn = 3600,
            User = new UserData
            {
                Id = DemoUserId,
                Email = email,
                Username = "demouser",
                Role = "Consumer"
            }
        };
    }

    public override async Task<bool> ValidateTokenAsync()
    {
        await Task.Delay(300);
        return true;
    }

    // ===== POST ENDPOINTS (Demo Data) =====
    public override Task<List<PostModel>> GetFeedAsync(int page = 1)
    {
        var posts = new List<PostModel>
        {
            new PostModel
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000101"),
                UserId = DemoUser2Id,
                Caption = "Beautiful sunset at the beach 🌅",
                ImageUrl = "https://images.unsplash.com/photo-1495567720989-cebdbdd97913?w=600&h=600&fit=crop",
                LikeCount = 245,
                CommentCount = 12,
                IsLikedByCurrentUser = false,
                User = new UserSummary
                {
                    Id = DemoUser2Id,
                    Username = "sarah_photos",
                    AvatarUrl = "https://i.pravatar.cc/150?img=1"
                },
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new PostModel
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000102"),
                UserId = DemoUser3Id,
                Caption = "Coffee and code ☕💻",
                ImageUrl = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=600&h=600&fit=crop",
                LikeCount = 129,
                CommentCount = 8,
                IsLikedByCurrentUser = true,
                User = new UserSummary
                {
                    Id = DemoUser3Id,
                    Username = "john_dev",
                    AvatarUrl = "https://i.pravatar.cc/150?img=2"
                },
                CreatedAt = DateTime.UtcNow.AddHours(-4)
            },
            new PostModel
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000103"),
                UserId = DemoUser2Id,
                Caption = "Mountain hiking adventure 🏔️",
                ImageUrl = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=600&h=600&fit=crop",
                LikeCount = 567,
                CommentCount = 34,
                IsLikedByCurrentUser = false,
                User = new UserSummary
                {
                    Id = DemoUser2Id,
                    Username = "sarah_photos",
                    AvatarUrl = "https://i.pravatar.cc/150?img=1"
                },
                CreatedAt = DateTime.UtcNow.AddHours(-8)
            },
            new PostModel
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000104"),
                UserId = DemoUser3Id,
                Caption = "First React project complete! 🎉",
                ImageUrl = "https://images.unsplash.com/photo-1633356122544-f134324ef6db?w=600&h=600&fit=crop",
                LikeCount = 89,
                CommentCount = 5,
                IsLikedByCurrentUser = false,
                User = new UserSummary
                {
                    Id = DemoUser3Id,
                    Username = "john_dev",
                    AvatarUrl = "https://i.pravatar.cc/150?img=2"
                },
                CreatedAt = DateTime.UtcNow.AddHours(-12)
            }
        };

        return Task.FromResult(posts);
    }

    public Task<PostModel?> GetPostAsync(Guid id)
    {
        var posts = GetPostsData();
        return Task.FromResult(posts.FirstOrDefault(p => p.Id == id));
    }

    public override Task<bool> CreatePostAsync(CreatePostModel model)
    {
        _logger.LogInformation("[MOCK] Created post: {title}", model.Title);
        return Task.FromResult(true);
    }

    public Task<bool> DeletePostAsync(Guid id)
    {
        _logger.LogInformation($"[MOCK] Deleted post: {id}");
        return Task.FromResult(true);
    }

    public Task<bool> LikePostAsync(Guid postId)
    {
        _logger.LogInformation($"[MOCK] Liked post: {postId}");
        return Task.FromResult(true);
    }

    public Task<bool> UnlikePostAsync(Guid postId)
    {
        _logger.LogInformation($"[MOCK] Unliked post: {postId}");
        return Task.FromResult(true);
    }

    // ===== COMMENT ENDPOINTS =====
    public override Task<List<CommentModel>> GetCommentsAsync(Guid postId)
    {
        var comments = new List<CommentModel>
        {
            new CommentModel
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                UserId = DemoUser3Id,
                Content = "Amazing shot!",
                User = new UserSummary
                {
                    Id = DemoUser3Id,
                    Username = "john_dev",
                    AvatarUrl = "https://i.pravatar.cc/150?img=2"
                },
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            },
            new CommentModel
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                UserId = DemoUserId,
                Content = "Love this! ❤️",
                User = new UserSummary
                {
                    Id = DemoUserId,
                    Username = "demouser",
                    AvatarUrl = "https://i.pravatar.cc/150?img=0"
                },
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };

        return Task.FromResult(comments);
    }

    public override Task<CommentModel?> AddCommentAsync(Guid postId, string content)
    {
        _logger.LogInformation($"[MOCK] Added comment to post {postId}: {content}");
        return Task.FromResult<CommentModel?>(new CommentModel
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            UserId = DemoUserId,
            Content = content,
            User = new UserSummary
            {
                Id = DemoUserId,
                Username = "demouser",
                AvatarUrl = "https://i.pravatar.cc/150?img=0"
            },
            CreatedAt = DateTime.UtcNow
        });
    }

    public override Task<PostRatingResponse?> RatePostAsync(Guid postId, int rating)
    {
        return Task.FromResult<PostRatingResponse?>(new PostRatingResponse
        {
            PostId = postId,
            AverageRating = rating,
            RatingCount = 1,
            CurrentUserRating = rating
        });
    }

    // ===== USER PROFILE ENDPOINTS =====
    public override Task<UserProfile?> GetUserProfileAsync(Guid userId)
    {
        var profile = new UserProfile
        {
            Id = userId,
            Email = "user@example.com",
            Username = userId == DemoUser2Id ? "sarah_photos" : userId == DemoUser3Id ? "john_dev" : "demouser",
            Bio = "Photography and coding enthusiast",
            AvatarUrl = userId == DemoUser2Id ? "https://i.pravatar.cc/150?img=1" : userId == DemoUser3Id ? "https://i.pravatar.cc/150?img=2" : "https://i.pravatar.cc/150?img=0",
            Role = "User",
            FollowerCount = 1250,
            FollowingCount = 340,
            PostCount = 42,
            IsFollowing = false,
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };

        return Task.FromResult((UserProfile?)profile);
    }

    public Task<bool> FollowUserAsync(Guid userId)
    {
        _logger.LogInformation($"[MOCK] Followed user: {userId}");
        return Task.FromResult(true);
    }

    public Task<bool> UnfollowUserAsync(Guid userId)
    {
        _logger.LogInformation($"[MOCK] Unfollowed user: {userId}");
        return Task.FromResult(true);
    }

    public override Task<List<UserProfile>> GetFollowersAsync(Guid userId)
    {
        var followers = new List<UserProfile>
        {
            new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = "follower1",
                AvatarUrl = "https://i.pravatar.cc/150?img=3",
                Bio = "Love photography"
            },
            new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = "follower2",
                AvatarUrl = "https://i.pravatar.cc/150?img=4",
                Bio = "Tech enthusiast"
            }
        };

        return Task.FromResult(followers);
    }

    // ===== SEARCH ENDPOINTS =====
    public Task<List<UserProfile>> SearchUsersAsync(string query)
    {
        var users = new List<UserProfile>
        {
            new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = "sarah_photos",
                AvatarUrl = "https://i.pravatar.cc/150?img=1",
                Bio = "Photography lover",
                FollowerCount = 1250
            },
            new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = "sarah_art",
                AvatarUrl = "https://i.pravatar.cc/150?img=5",
                Bio = "Digital artist",
                FollowerCount = 890
            }
        };

        return Task.FromResult(users);
    }

    // Helper method
    private List<PostModel> GetPostsData()
    {
        return new List<PostModel>
        {
            new PostModel
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000101"),
                UserId = DemoUser2Id,
                Caption = "Beautiful sunset at the beach 🌅",
                ImageUrl = "https://images.unsplash.com/photo-1495567720989-cebdbdd97913?w=600&h=600&fit=crop",
                LikeCount = 245,
                CommentCount = 12,
                IsLikedByCurrentUser = false,
                User = new UserSummary { Id = DemoUser2Id, Username = "sarah_photos" },
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            }
        };
    }
}
