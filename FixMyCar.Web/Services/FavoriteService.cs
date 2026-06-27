using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;

namespace FixMyCar.Web.Services;

public class FavoriteService(IFavoriteRepository favoriteRepository) : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository = favoriteRepository;

    public async Task ToggleAsync(int userId, int postId)
    {
        var favorite = await _favoriteRepository.GetAsync(userId, postId);

        if (favorite is null)
        {
            await _favoriteRepository.AddAsync(new Favorite
            {
                UserId = userId,
                PostId = postId
            });
        }
        else
            await _favoriteRepository.RemoveAsync(favorite);

        await _favoriteRepository.SaveChangesAsync();
    }
    public async Task<List<int>> GetFavoritePostIdsAsync(int userId)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(userId);

        return favorites
            .Select(f => f.PostId)
            .ToList();
    }

    public async Task<bool> IsFavoriteAsync(int userId, int postId)
        => await _favoriteRepository.GetAsync(userId, postId) != null;

    public async Task<List<Favorite>> GetUserFavoritesAsync(int userId)
        => await _favoriteRepository.GetByUserIdAsync(userId);
}