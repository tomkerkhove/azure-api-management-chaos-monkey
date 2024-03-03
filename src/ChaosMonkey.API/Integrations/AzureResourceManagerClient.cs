using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ApiManagement;

namespace ChaosMonkey.API.Integrations
{
    public class AzureResourceManagerClient
    {
        readonly ArmClient _armClient;

        public AzureResourceManagerClient()
        {
            var auth = new DefaultAzureCredential();
            _armClient = new ArmClient(auth);
        }
    
        public AzureResourceManagerClient(string tenantId, string appId, string appSecret)
        {
            var auth = new ClientSecretCredential(tenantId, appId, appSecret);
            _armClient = new ArmClient(auth);
        }

        public async Task<ApiManagementServiceData?> GetServiceInfo(string subscriptionId, string resourceGroupName, string serviceName)
        {
            var armResourceId = ApiManagementServiceResource.CreateResourceIdentifier(subscriptionId, resourceGroupName, serviceName);
            var apiManagementServiceResource = _armClient.GetApiManagementServiceResource(armResourceId);
            var serviceInfoResponse = await apiManagementServiceResource.GetAsync();
            return serviceInfoResponse.Value?.Data;
        }
    }
}