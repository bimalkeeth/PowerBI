using Newtonsoft.Json;

namespace PowerBIService.Common.Contracts.GenericContracts
{
    public interface IPBIObject
    {
        [JsonIgnore]
        string ApiURL { get; }
        [JsonIgnore]
        PBIGroup ParentGroup { get; }

        [JsonIgnore]
        IPBIObject ParentObject { get; }

        [JsonIgnore]
        PBIAPIClient ParentPowerBIAPI { get; set; }

        string Id { get; set; }
    }
}