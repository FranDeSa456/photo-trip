using AutoMapper;
using PhotoTrip.BLL.Models;
using PhotoTrip.BLL.Services.Interfaces;
using PhotoTrip.DAL.Entities;
using PhotoTrip.DAL.Repositories.Interfaces;

namespace PhotoTrip.BLL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IReviewRepository reviewRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IReadOnlyList<ReviewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var reviews = await _reviewRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<ReviewModel>>(reviews);
        }

        public async Task<ReviewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(id, cancellationToken);
            return review is null ? null : _mapper.Map<ReviewModel>(review);
        }

        public async Task<ReviewModel> CreateAsync(ReviewModel reviewModel, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(reviewModel);
            var review = _mapper.Map<Review>(reviewModel);
            await _reviewRepository.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ReviewModel>(review);
        }

        public async Task<bool> UpdateAsync(ReviewModel reviewModel, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(reviewModel);
            var review = _mapper.Map<Review>(reviewModel);
            _reviewRepository.Update(review);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(id, cancellationToken);
            if (review is null)
                return false;

            _reviewRepository.Remove(review);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}