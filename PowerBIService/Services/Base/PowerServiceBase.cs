using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;
using PowerBIService.Common;

namespace PowerBIService.Services.Base
{
    public abstract class PowerServiceBase
    {
        #region Base Variables
        protected UserData UserCredential { get; set; }
        protected AuthenticationResult TokenResult { get; set; }
        protected TokenCredentials PTokenCredentials { get; set;}
        
        
        protected static string POWER_BI_API_URL = "https://api.powerbi.com";
        private static string POWER_BI_AUTHORITY_URL ="https://login.microsoftonline.com/{0}/oauth2/v2.0/token";
        private static string POWER_BI_RESOURCE_URL = "https://analysis.windows.net/powerbi/api";
       
        #endregion
        #region Authontication
        
        /// <summary>
        /// This method uses Service Principle approach to gives the access token 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AdalSilentTokenAcquisitionException"></exception>
        /// <exception cref="Exception"></exception>
        protected  async Task<bool> AuthenticateAsync()
        {
            
            TokenCache TC = new TokenCache();
            var clientCredential = new ClientCredential(UserCredential.ApplicationId, UserCredential.SecretId);
            AuthenticationContext ctx;
            
            if (!string.IsNullOrEmpty(UserCredential.TenantId))
            {
                ctx=new AuthenticationContext(string.Format(POWER_BI_AUTHORITY_URL,UserCredential.TenantId),false,TC);
            }
            else
            {
                ctx=new AuthenticationContext(string.Format(POWER_BI_AUTHORITY_URL,UserCredential.TenantId)+"/common");
                if (ctx.TokenCache.Count > 0)
                {
                    var cacheToken = ctx.TokenCache.ReadItems().First().TenantId;
                    ctx = new AuthenticationContext(string.Format(POWER_BI_AUTHORITY_URL,UserCredential.TenantId)+cacheToken ,false,TC);
                }
            }
            try
            {
                TokenResult = await ctx.AcquireTokenAsync(POWER_BI_RESOURCE_URL,clientCredential);
                PTokenCredentials=new TokenCredentials(TokenResult.AccessToken,TokenResult.AccessTokenType);
                return true;
            }
            catch (AdalSilentTokenAcquisitionException silentExp)
            {
                throw silentExp;
            }
            catch (Exception genExp)
            {
                throw genExp;
            }
        }
        #endregion
        
        
    }
}