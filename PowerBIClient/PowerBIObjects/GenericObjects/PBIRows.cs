﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GB.PowerBI.API.PowerBIObjects
{
    [JsonArray]
    public class PBIRows : PBIObjectList<PBIRow>
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
