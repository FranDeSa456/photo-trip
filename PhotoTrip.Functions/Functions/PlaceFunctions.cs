using Microsoft.Azure.Functions.Worker;
using System.Net;
using PhotoTrip.BLL.Services.Interfaces;

namespace PhotoTrip.Functions.Functions
{
    public class PlaceFunctions
    {
        private readonly IPlaceService _placeService;

        public PlaceFunctions(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        // GET /api/regioni/{regionId}/luoghi
        [Function("GetPlacesByRegion")]
        public async Task<HttpResponseData> GetByRegion(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "regioni/{regionId:int}/luoghi")] HttpRequestData req,
            int regionId)
        {
            var places = await _placeService.GetByRegionAsync(regionId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(places);
            return response;
        }

        // GET /api/luoghi/{id}
        [Function("GetPlaceDetail")]
        public async Task<HttpResponseData> GetDetail(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "luoghi/{id:int}")] HttpRequestData req,
            int id)
        {
            var place = await _placeService.GetDetailAsync(id);
            if (place is null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(place);
            return response;
        }

        // DELETE /api/luoghi/{id}
        [Function("DeletePlace")]
        public async Task<HttpResponseData> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "luoghi/{id:int}")] HttpRequestData req,
            int id)
        {
            var deleted = await _placeService.RemoveAsync(id);
            if (!deleted)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            await response.WriteStringAsync(string.Empty);
            return response;
        }
    }
}
