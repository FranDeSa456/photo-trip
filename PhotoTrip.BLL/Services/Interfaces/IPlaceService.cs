using PhotoTrip.BLL.Models;

namespace PhotoTrip.BLL.Services.Interfaces
{
    internal interface IPlaceService
    {
        Task<PlaceModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PlaceModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PlaceModel> AddAsync(PlaceModel model, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(PlaceModel model, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default);
    }
}
