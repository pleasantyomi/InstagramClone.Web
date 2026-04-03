namespace InstagramClone.Web.Models;

public class LoginModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterModel
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public int ExpiresIn { get; set; }
    public UserData? User { get; set; }
}

public class UserData
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class PostModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? PeoplePresent { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public decimal AverageRating { get; set; }
    public int RatingCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public UserSummary? User { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserSummary
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}

public class CommentModel
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public UserSummary? User { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserProfile
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public int PostCount { get; set; }
    public bool IsFollowing { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePostModel
{
    public string Title { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? PeoplePresent { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class PostRatingResponse
{
    public Guid PostId { get; set; }
    public decimal AverageRating { get; set; }
    public int RatingCount { get; set; }
    public int? CurrentUserRating { get; set; }
}
