using FixMyCar.Web.Models;

namespace FixMyCar.Web.Services;

public interface IFavoriteService
{
    Task ToggleAsync(int userId, int postId);

    Task<bool> IsFavoriteAsync(int userId, int postId);

    Task<List<Favorite>> GetUserFavoritesAsync(int userId);
    Task<List<int>> GetFavoritePostIdsAsync(int userId);
}
