using PhotoTrip.DAL.Entities;

namespace PhotoTrip.DAL.Repositories.Interfaces
{
    internal interface IPlaceRepository
    {
        Task<Place?> GetByIdAsync(int Id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Place?>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Place entity, CancellationToken cancellationToken = default);
        void Update(Place entity);
        void Remove(Place entity);
    }
}
