using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PowerBIService.Helpers
{
    public static class PBIJsonHelper
    {
         public static string SerializeObject(object objectToSerialize)
         {
                var settings = new JsonSerializerSettings { ContractResolver = new JsonIgnoreSerializeContractResolver(), NullValueHandling = NullValueHandling.Ignore };
                string json = JsonConvert.SerializeObject(objectToSerialize, settings);
                return json;
         }
    }
    [AttributeUsage(AttributeTargets.Property)]
    class JsonIgnoreSerialize : Attribute { }
    
    public class JsonIgnoreSerializeContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            Type classType = null;
            var property = base.CreateProperty(member, memberSerialization);
            if (property != null && property.Writable)
            {
                var attributes = property.AttributeProvider.GetAttributes(typeof(JsonIgnoreSerialize), true);
                if (attributes != null && attributes.Count > 0)
                {
                    property.Writable = false;
                    property.Ignored = true;
                }
            }
            //------------------------------------------------------------------------------------------- 
            // dynamically get the corresponding PBI-API object if a property from a PBIv2 is serialized
            //-------------------------------------------------------------------------------------------
            classType = Type.GetType(GetType().Namespace + ".PowerBIObjects.PBI" + member.DeclaringType.Name);
            if(classType == null)
                classType = member.DeclaringType;

            var method = classType.GetMethod("ShouldSerialize_" + property.PropertyName);

            if (method != null && method.ReturnType == typeof(bool))
            {
                property.ShouldSerialize = instance =>
                {
                    var shouldSerialize = method.Invoke(instance, null) as bool?;
                    return shouldSerialize ?? true;
                };
            }

            return property;
        }
    }
}