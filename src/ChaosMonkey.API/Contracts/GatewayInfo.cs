namespace ChaosMonkey.API.Contracts
{
    public class GatewayInfo
    {
        public GatewayType? Type { get; set; }
        public string Region { get; set; }
        public Uri RegionalUrl { get; set; }
        public bool IsEnabled { get; set; }
        public List<string>? AvailabilityZones { get; set; } = new List<string>();
    }
}