using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using PhotoTrip.BLL.Models;
using PhotoTrip.BLL.Services.Interfaces;
using System.Net;

namespace PhotoTrip.Functions.Functions
{
    public class ReviewFunctions
    {
        private readonly IReviewService _reviewService;
        private readonly IPlaceService _placeService;

        public ReviewFunctions(IReviewService reviewService, IPlaceService placeService)
        {
            _reviewService = reviewService;
            _placeService = placeService;
        }
        
        [Function("GetReviewsByPlace")]
        public async Task<HttpResponseData> GetReviewsByPlace(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "luoghi/{luogoId:int}/recensioni")] HttpRequestData req,
            int luogoId)
        {
            var place = await _placeService.GetByIdAsync(luogoId);
            if (place is null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            
            var reviews = await _reviewService.GetByPlaceIdAsync(luogoId);
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(reviews);
            return response;
        }
        
        [Function("CreateReview")]
        public async Task<HttpResponseData> CreateReview(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "luoghi/{luogoId:int}/recensioni")] HttpRequestData req,
            int luogoId)
        {  
            var place = await _placeService.GetByIdAsync(luogoId);
            if (place is null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            
            var model = await req.ReadFromJsonAsync<ReviewModel>();
            if (model is null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            
            model.PlaceId = luogoId;
            model.Date = DateTime.UtcNow;
            
            var createdReview = await _reviewService.CreateAsync(model);
            
            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(createdReview);
            return response;
        }
        
        [Function("DeleteReview")]
        public async Task<HttpResponseData> DeleteReview(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "luoghi/{luogoId:int}/recensioni/{id:int}")] HttpRequestData req,
            int luogoId,
            int id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review is null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            
            if (review.PlaceId != luogoId)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            
            await _reviewService.DeleteAsync(id);

            return req.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}