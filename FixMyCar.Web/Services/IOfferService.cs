using FixMyCar.Web.Models;

namespace FixMyCar.Web.Services;

public interface IOfferService
{
    Task<List<Offer>> GetAllAsync();

    Task<Offer?> GetByIdAsync(int id);

    Task<List<Offer>> GetByPostIdAsync(int postId);

    Task CreateAsync(Offer offer);
    Task<List<Offer>> GetByUserIdAsync(int userId);
    Task DeleteAsync(int id);
    Task UpdateAsync(Offer offer);
}