using System;
using Microsoft.PowerBI.Api.V2.Models;
using Newtonsoft.Json;
using PowerBIService.Common.Contracts.GenericContracts;
using PowerBIService.Helpers;

namespace PowerBIService.Common.Contracts
{
    public class PBIColumn: Column, IPBIObject
    {
        #region Constructors
        public PBIColumn(string name, PbiDataTypeEnum dataType)
        {
            Name = name;
            DataType = dataType.ToString();
        }
        #endregion
        
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "dataCategory", NullValueHandling = NullValueHandling.Ignore)]
        private string _dataCategoryString;
        
        [JsonProperty(PropertyName = "summarizeBy", NullValueHandling = NullValueHandling.Ignore)]
        private string _summarizeByString;
        #endregion
        
        #region Public Properties
        [JsonProperty(PropertyName = "formatString", NullValueHandling = NullValueHandling.Ignore)]
        public string FormatString { get; set; }
        
        
        [JsonProperty(PropertyName = "sortByColumn", NullValueHandling = NullValueHandling.Ignore)]
        public string SortByColumn { get; set; }
        
        [JsonProperty(PropertyName = "isHidden", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsHidden { get; set; }
        
        [JsonIgnore]
        public PbiDataTypeEnum PBIDataType
        {
            get => (PbiDataTypeEnum)Enum.Parse(typeof(PbiDataTypeEnum), DataType, true);
            set => DataType = value.ToString();
        }
        
        [JsonIgnore]
        public PbiDataCategoryEnum? DataCategory
        {
            get
            {
                if (string.IsNullOrEmpty(_dataCategoryString))
                    return null;
                return (PbiDataCategoryEnum)Enum.Parse(typeof(PbiDataCategoryEnum), _dataCategoryString, true);
            }
            set => _dataCategoryString = value == null ? null : value.ToString();
        }
        
        [JsonIgnore]
        public PbiSummarizeByEnum? PBISummarizeBy
        {
            get
            {
                if (string.IsNullOrEmpty(_summarizeByString))
                    return null;

                return (PbiSummarizeByEnum)Enum.Parse(typeof(PbiSummarizeByEnum), _summarizeByString, true);
            }
            set => _summarizeByString = value == null ? null : value.ToString();
        }
        
        [JsonIgnore]
        public PBITable ParentTable { get; set; }
        public string ApiURL => null;

        public PBIGroup ParentGroup => ((IPBIObject)ParentTable).ParentGroup;

        public IPBIObject ParentObject => ((IPBIObject)ParentTable).ParentObject;


        public PBIAPIClient ParentPowerBIAPI
        {
            get => ((IPBIObject)ParentTable).ParentPowerBIAPI;
            set => throw new NotImplementedException("Cannot set the ParentPowerBiApi of a Column. It is always derived from its ParentTable!");
        }
        public string Id
        {
            get => null;
            set
            {
            }
        }
        #endregion
        #region ShouldSerialize-Functions
        
        public bool ShouldSerialize_formatString()
        {
            if (ParentTable == null || ParentTable.ParentDataset == null || ParentTable.ParentDataset.PBIDefaultMode == PbiDefaultModeEnum.Streaming)
            {
                if (!string.IsNullOrEmpty(FormatString))
                    PowerBIHelper.WriteWarning("FormatStrings are not supported in Streaming-Mode (column [{0}])!", Name);
                return false;
            }

            return true;
        }
        public bool ShouldSerialize_sortByColumn()
        {
            if (ParentTable == null || ParentTable.ParentDataset == null || ParentTable.ParentDataset.PBIDefaultMode == PbiDefaultModeEnum.Streaming)
            {
                if (!string.IsNullOrEmpty(SortByColumn))
                    PowerBIHelper.WriteWarning("SortByColumns are not supported in Streaming-Mode (column [{0}])!", Name);
                return false;
            }

            return true;
        }
        public bool ShouldSerialize_isHidden()
        {
            if (ParentTable == null || ParentTable.ParentDataset == null || ParentTable.ParentDataset.PBIDefaultMode == PbiDefaultModeEnum.Streaming)
            {
                if (IsHidden.HasValue)
                    PowerBIHelper.WriteWarning("IsHidden is not supported in Streaming-Mode (column [{0}])!", Name);
                return false;
            }

            return true;
        }
        public bool ShouldSerialize_dataCategory()
        {
            if (ParentTable == null || ParentTable.ParentDataset == null || ParentTable.ParentDataset.PBIDefaultMode == PbiDefaultModeEnum.Streaming)
            {
                if (DataCategory.HasValue)
                    PowerBIHelper.WriteWarning("DataCategories are not supported in Streaming-Mode (column [{0}])!", Name);
                return false;
            }

            return true;
        }
        
        public bool ShouldSerialize_summarizeBy()
        {
            if (ParentTable == null || ParentTable.ParentDataset == null || ParentTable.ParentDataset.PBIDefaultMode == PbiDefaultModeEnum.Streaming)
            {
                if (PBISummarizeBy.HasValue)
                    PowerBIHelper.WriteWarning("SummarizeBy is not supported in Streaming-Mode (column [{0}])!", Name);
                return false;
            }

            return true;
        }
        
        #endregion
        
        
    }
}