using AutoMapper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using PhotoTrip.BLL.Models;
using PhotoTrip.BLL.Services.Interfaces;
using System.Net;

namespace PhotoTrip.Functions.Functions
{
    public class RegionFunctions(IRegionService regionService, IMapper mapper)
    {
        private readonly IRegionService _regionService = regionService;
        private readonly IMapper _mapper = mapper;

        [Function("GetRegions")]
        public async Task<HttpResponseData> GetRegions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "regioni")] HttpRequestData req)
        {
            var regions = await _regionService.GetAllAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(_mapper.Map<IEnumerable<RegionModel>>(regions));
            return response;
        }
    }
}
