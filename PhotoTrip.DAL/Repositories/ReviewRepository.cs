using Microsoft.EntityFrameworkCore;
using PhotoTrip.DAL.Data;
using PhotoTrip.DAL.Entities;
using PhotoTrip.DAL.Repositories.Interfaces;

namespace PhotoTrip.DAL.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        protected readonly PhotoDbContext _context;
        protected readonly DbSet<Review> _dbSet;
        public ReviewRepository(PhotoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<Review>();
        }

        public async Task<IReadOnlyList<Review>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public async Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }

        public async Task AddAsync(Review entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(Review entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _dbSet.Update(entity);
        }

        public void Remove(Review entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _dbSet.Remove(entity);
        }
    }
}
