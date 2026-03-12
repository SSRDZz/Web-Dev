using KMITL_WebDev_MiniProject.Entites;

namespace KMITL_WebDev_MiniProject.Models;

public class PopularViewModel
{
    public List<PopularActivityItem> PopularPosts { get; set; } = new();
    public List<PopularUserItem> PopularPeople { get; set; } = new();
}

public class PopularActivityItem
{
    public Activity Activity { get; set; } = null!;
    public string OwnerName { get; set; } = string.Empty;
    public int LikeCount { get; set; }
}

public class PopularUserItem
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ImagePath { get; set; } = "image/UserProfile/guest_picture.jpg";
    public int ReputationScore { get; set; }
    public int PositiveVotes { get; set; }
}
