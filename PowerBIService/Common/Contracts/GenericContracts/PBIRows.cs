using System.Collections.Generic;
using Newtonsoft.Json;
using PowerBIService.Helpers;

namespace PowerBIService.Common.Contracts.GenericContracts
{
    public class PBIRows:PBIObjectList<PBIRow>
    {
          #region Private Properties for Serialization
                [JsonProperty(PropertyName = "rows", NullValueHandling = NullValueHandling.Ignore)]
                public List<PBIRow> Rows { get { return Items; } set { Items = value; } }
        
                [JsonIgnore]
                public PBITable ParentTable { get; set; }
        
                [JsonIgnore]
                public string JSON { get { return PBIJsonHelper.SerializeObject(this); } }
                #endregion
    }
}