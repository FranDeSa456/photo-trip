using PhotoTrip.BLL.Models;

namespace PhotoTrip.BLL.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IReadOnlyList<ReviewModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ReviewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ReviewModel> CreateAsync(ReviewModel reviewModel, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(ReviewModel reviewModel, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}