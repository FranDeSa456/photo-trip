using Microsoft.EntityFrameworkCore;
using PhotoTrip.DAL.Data;
using PhotoTrip.DAL.Entities;
using PhotoTrip.DAL.Repositories.Interfaces;

namespace PhotoTrip.DAL.Repositories
{
    public class PlaceRepository(PhotoDbContext context) : IPlaceRepository
    {
        private readonly DbSet<Place> _dbSet = context.Set<Place>();

        public async Task<Place?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }

        public async Task<IReadOnlyList<Place?>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Place entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(Place entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(Place entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
