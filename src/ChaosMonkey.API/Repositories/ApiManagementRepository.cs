using Azure.Core;
using Azure.ResourceManager.ApiManagement.Models;
using ChaosMonkey.API.Contracts;
using ChaosMonkey.API.Integrations;

namespace ChaosMonkey.API.Repositories
{
    public class ApiManagementRepository
    {
        readonly AzureResourceManagerClient _armClient;

        public ApiManagementRepository(AzureResourceManagerClient armClient)
        {
            _armClient = armClient;
        }

        public async Task ManageGatewayInRegion(string subscriptionId, string resourceGroupName, string serviceName, AzureLocation azureRegionInfo, bool isGatewayEnabled)
        {
            var serviceInfo = await this._armClient.GetServiceInfo(subscriptionId, resourceGroupName, serviceName);

            var secondaryLocationInRegion = serviceInfo?.AdditionalLocations.FirstOrDefault(x => x.Location == azureRegionInfo);
            if (serviceInfo?.Location == azureRegionInfo)
            {
                await this._armClient.UpdateService(subscriptionId, resourceGroupName, serviceName, patch => patch.DisableGateway = isGatewayEnabled);                
            }
            else if (secondaryLocationInRegion != null)
            {
                await this._armClient.UpdateService(subscriptionId, resourceGroupName, serviceName, patch =>
                {
                    foreach (var matchingSecondaryLocation in patch.AdditionalLocations.Where(x => x.Location == azureRegionInfo))
                    {
                        matchingSecondaryLocation.DisableGateway = isGatewayEnabled;
                    }
                });
            }
            else
            {
                throw new NotSupportedException("Region not found");
            }
        }

        public async Task<List<GatewayInfo>> Get(string subscriptionId, string resourceGroupName, string serviceName)
        {
            var serviceInfo = await this._armClient.GetServiceInfo(subscriptionId, resourceGroupName, serviceName);

            var gatewayInfo = new List<GatewayInfo>
            {
                new GatewayInfo
                {
                    Type = GatewayType.Primary,
                    Region = serviceInfo.Location,
                    IsEnabled = !serviceInfo.DisableGateway ?? true,
                    RegionalUrl = serviceInfo.GatewayRegionalUri,
                    AvailabilityZones = serviceInfo.Zones?.ToList()
                }
            };

            foreach (var additionalLocation in serviceInfo.AdditionalLocations)
            {
                var gatewayInfoEntry = new GatewayInfo
                {
                    Type = GatewayType.Secondary,
                    Region = additionalLocation.Location,
                    IsEnabled = !additionalLocation.DisableGateway ?? true,
                    RegionalUrl = additionalLocation.GatewayRegionalUri,
                    AvailabilityZones = additionalLocation.Zones?.ToList()
                };
                
                gatewayInfo.Add(gatewayInfoEntry);
            }

            return gatewayInfo;
        }
    }
}