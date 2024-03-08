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