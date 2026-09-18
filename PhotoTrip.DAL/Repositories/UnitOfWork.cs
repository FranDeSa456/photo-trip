using PhotoTrip.DAL.Data;
using PhotoTrip.DAL.Repositories.Interfaces;

namespace PhotoTrip.DAL.Repositories
{
    public class UnitOfWork(PhotoDbContext context) : IUnitOfWork
    {
        private readonly PhotoDbContext _context = context;
        private bool _disposed;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _context.Dispose();
            _disposed = true;
        }
    }
}
