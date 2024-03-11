using Azure.Core;
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

        [HttpPut("api/v1/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/service/{serviceName}/locations/manage")]
        public async Task<ObjectResult> DisableGateway(string subscriptionId, string resourceGroupName, string serviceName, [FromBody] GatewayState gatewayState)
        {
            var regionName = gatewayState?.RegionName;
            var azureRegionInfo = new AzureLocation(regionName);
            if (string.IsNullOrWhiteSpace(azureRegionInfo.DisplayName))
            {
                return BadRequest(new { message = $"Region '{regionName}' is not a supported region" });
            }

            await this._apimRepository.ManageGatewayInRegion(subscriptionId, resourceGroupName, serviceName, azureRegionInfo, gatewayState.IsEnabled);
            return Accepted();
        }
    }

    public class GatewayState
    {
        public string RegionName { get; set; }
        public bool IsEnabled { get; set; }
    }
}