using AutoMapper;
using PhotoTrip.BLL.Models;
using PhotoTrip.BLL.Services.Interfaces;
using PhotoTrip.DAL.Entities;
using PhotoTrip.DAL.Repositories.Interfaces;

namespace PhotoTrip.BLL.Services
{
    public class PlaceService(IUnitOfWork unitOfWork, IPlaceRepository repository, IMapper mapper) : IPlaceService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPlaceRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PlaceModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            return entity is null ? default : _mapper.Map<PlaceModel>(entity);
        }

        public async Task<IReadOnlyList<PlaceModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<PlaceModel>>(entities);
        }

        public async Task<PlaceModel> AddAsync(PlaceModel model, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Place>(model);
            await _repository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PlaceModel>(entity);
        }

        public async Task<bool> UpdateAsync(PlaceModel model, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Place>(model);
            _repository.Update(entity);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        public async Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity is null)
                return false;

            _repository.Remove(entity);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
