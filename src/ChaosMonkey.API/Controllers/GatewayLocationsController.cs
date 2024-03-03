using ChaosMonkey.API.Contracts;
using ChaosMonkey.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChaosMonkey.API.Controllers
{
    [ApiController]
    public class GatewayLocationsController : Controller
    {
        readonly ApiManagementRepository _apimRepository;

        public GatewayLocationsController(ApiManagementRepository apimRepository)
        {
            _apimRepository = apimRepository;
        }

        [HttpGet("api/v1/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/service/{serviceName}/locations")]
        [ProducesResponseType<List<GatewayInfo>>(StatusCodes.Status200OK)]
        public async Task<List<GatewayInfo>> Get(string subscriptionId, string resourceGroupName, string serviceName)
        {
            var gatewayInfo = await this._apimRepository.Get(subscriptionId, resourceGroupName, serviceName);
            return gatewayInfo;
        }
    }
}