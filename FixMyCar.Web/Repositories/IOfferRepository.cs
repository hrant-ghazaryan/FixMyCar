using FixMyCar.Web.Models;

namespace FixMyCar.Web.Repositories;

public interface IOfferRepository
{
    Task<List<Offer>> GetAllAsync();

    Task<Offer?> GetByIdAsync(int id);

    Task<List<Offer>> GetByPostIdAsync(int postId);

    Task AddAsync(Offer offer);

    void Delete(Offer offer);
    Task<List<Offer>> GetByUserIdAsync(int userId);

    Task SaveAsync();
}