using System;
using Microsoft.PowerBI.Api.V2.Models;

namespace PowerBIService.Common
{
    /// <summary>-----------------------------------------------
    /// Class to hold configuration information
    /// </summary>----------------------------------------------
    public class EmbedConfig
    {
        public string Id { get; set; }

        public string EmbedUrl { get; set; }

        public EmbedToken EmbedToken { get; set; }

        public int MinutesToExpiration
        {
            get
            {
                if (EmbedToken.Expiration != null)
                {
                    var minutesToExpiration = EmbedToken.Expiration.Value - DateTime.UtcNow;
                    return minutesToExpiration.Minutes;
                }
                return 5;
            }
        }
        public bool? IsEffectiveIdentityRolesRequired { get; set; }

        public bool? IsEffectiveIdentityRequired { get; set; }

        public bool EnableRLS { get; set; }

        public string Username { get; set; }

        public string Roles { get; set; }

        public string ErrorMessage { get; internal set; }
    }
}