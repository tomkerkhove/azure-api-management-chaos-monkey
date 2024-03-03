using System.Text.Json.Serialization;

namespace ChaosMonkey.API.Contracts
{
    [JsonConverter(typeof(JsonStringEnumConverter<GatewayType>))]

    public enum GatewayType
    {
        Unknown,
        Primary,
        Secondary
    }
}