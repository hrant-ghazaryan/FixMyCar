using FixMyCar.Web.Models;

namespace FixMyCar.Web.Repositories;

public interface IFavoriteRepository
{
    Task AddAsync(Favorite favorite);
    Task RemoveAsync(Favorite favorite);
    Task<Favorite?> GetAsync(int userId, int postId);
    Task<List<Favorite>> GetByUserIdAsync(int userId);
    Task SaveChangesAsync();
}
