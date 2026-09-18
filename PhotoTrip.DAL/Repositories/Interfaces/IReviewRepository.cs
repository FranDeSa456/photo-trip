using PhotoTrip.DAL.Entities;

namespace PhotoTrip.DAL.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<IReadOnlyList<Review>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task AddAsync(Review entity, CancellationToken cancellationToken = default);

        void Update(Review entity);

        void Remove(Review entity);
    }
}
