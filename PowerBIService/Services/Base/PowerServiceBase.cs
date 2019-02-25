using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
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
        private static string   POWER_BI_AUTHORITY_URL ="https://login.microsoftonline.com/{0}/oauth2/v2.0/token";
        private static string   POWER_BI_RESOURCE_URL = "https://analysis.windows.net/powerbi/api";
        protected TokenCache TC;
        #endregion

        protected PowerServiceBase()
        {
             TC = new TokenCache();
        }
        
        
        #region Authontication
        
        /// <summary>
        /// This method uses Service Principle approach to gives the access token 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AdalSilentTokenAcquisitionException"></exception>
        /// <exception cref="Exception"></exception>
        protected  async Task<bool> AuthenticateAsync()
        {
            
            
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
                    ctx = new AuthenticationContext(string.Format(POWER_BI_AUTHORITY_URL,UserCredential.TenantId)+"/"+cacheToken ,false,TC);
                }
            }
            try
            {
                ctx.ExtendedLifeTimeEnabled = true;
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
        #region RestApi Functions
        
        /// <summary>-------------------------------------------
        /// Send Async call to power Bi Service 
        /// </summary>------------------------------------------
        /// <param name="endpoint"></param>
        /// <param name="httpMethod"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="HttpOperationException"></exception>
        private async Task<HttpResponseMessage> SendAsync(string endpoint, string httpMethod, HttpContent content = null)
        {
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), endpoint) { };

            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(900000)
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TokenResult.AccessToken);
            if (content != null)
            {
                request.Content = content;
            }
            var response = await client.SendAsync(request, default(CancellationToken));
            if (!response.IsSuccessStatusCode)
            {
                var exc = new HttpOperationException()
                {
                    Response = new HttpResponseMessageWrapper(response, await response.Content.ReadAsStringAsync())
                };
                throw exc;
            }
            return response;
        }
        /// <summary>---------------------------------------
        /// Form Stream of report to copy before posting
        /// </summary>--------------------------------------
        /// <param name="workspaceId"></param>
        /// <param name="content"></param>
        /// <param name="datasetDisplayName"></param>
        /// <returns></returns>
        private async Task<Import> PostImportAsync(string workspaceId, Stream content, string datasetDisplayName)
        {
            var displayNameQueryParameter = (!string.IsNullOrEmpty(datasetDisplayName))
                ? FormattableString.Invariant($"?datasetDisplayName={Uri.EscapeDataString(datasetDisplayName)}")
                : string.Empty;
            string postImportEndpoint = FormattableString.Invariant($"https://api.powerbi.com/v1.0/myorg/groups/{workspaceId}/imports{displayNameQueryParameter}");

            var contentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data") {Name = "file0"};
            HttpContent fileStreamContent = new StreamContent(content);
            fileStreamContent.Headers.ContentDisposition = contentDispositionHeaderValue;

            var multiPartContent = new MultipartFormDataContent {fileStreamContent};

            var response = await SendAsync(postImportEndpoint, "POST", multiPartContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            return SafeJsonConvert.DeserializeObject<Import>(responseContent);
        }
        
        protected async Task<Import> TryUploadAsync(IPowerBIClient client, string workspaceId, Stream content, string datasetDisplayName)
        {
            var import = await PostImportAsync(workspaceId, content, datasetDisplayName);
            while (import.ImportState != "Succeeded" && import.ImportState != "Failed")
            {
                import = await client.Imports.GetImportByIdInGroupAsync(workspaceId, import.Id);
            }
            if (import.ImportState != "Succeeded")
            {
                return null;
            }
            return import;
        }
        
        /// <summary>----------------------------------------------
        /// Set Report Parameters to update reports
        /// </summary>---------------------------------------------
        /// <param name="client"></param>
        /// <param name="tenantWorkspaceId"></param>
        /// <param name="reportDatasetId"></param>
        /// <param name="reportParameters"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected static async Task SetReportParameters(IPowerBIClient client, string tenantWorkspaceId, string reportDatasetId, IEnumerable<DatasetParameter> reportParameters, IDictionary<string, string> parameters)
        {
            if (reportParameters != null && reportParameters.Any())
            {
                if ((parameters == null || !parameters.Any()) && reportParameters.Any(x => x.IsRequired) ||
                    (parameters != null || parameters.Any()) && reportParameters.Any(x => x.IsRequired && !parameters.Keys.Contains(x.Name)))
                {
                    throw new Exception("RequiredParameterValueMissing");
                }

                if (parameters.Any())
                {
                    var updateParameterList = new List<UpdateDatasetParameterDetails>();
                    foreach (var reportParameter in reportParameters)
                    {
                        if (parameters.ContainsKey(reportParameter.Name))
                        {
                            updateParameterList.Add(new UpdateDatasetParameterDetails { Name = reportParameter.Name, NewValue = parameters[reportParameter.Name] });
                        }
                    }
                    if (updateParameterList.Any())
                    {
                        await client.Datasets.UpdateParametersInGroupAsync(tenantWorkspaceId, reportDatasetId, new UpdateDatasetParametersRequest
                            {
                                UpdateDetails = updateParameterList
                            }
                        );
                    }
                }
            }
        }
        protected async Task PostDataset(string groupId,string rawJson)
        {
            var powerBIApiAddRowsUrl =$"https://api.powerbi.com/v1.0/myorg/groups/{groupId}/datasets";
            var byteArray = Encoding.UTF8.GetBytes(rawJson);
            var request2 = new HttpRequestMessage(new HttpMethod("POST"), powerBIApiAddRowsUrl) { };
            Stream stream = new MemoryStream(byteArray);
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(900000)
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TokenResult.AccessToken);
            
            var contentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data") {Name = "file0"};
            HttpContent fileStreamContent = new StreamContent(stream);
            fileStreamContent.Headers.ContentDisposition = contentDispositionHeaderValue;

            var multiPartContent = new MultipartFormDataContent {fileStreamContent};
            if (stream != null)
            {
                request2.Content = multiPartContent;
            }
            var response = await client.SendAsync(request2, default(CancellationToken));
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var exc = new HttpOperationException
                    {
                        Response = new HttpResponseMessageWrapper(response, await response.Content.ReadAsStringAsync())
                    };
                    throw exc;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
              
            }
        }
        static async Task<HttpResponseMessage> PostAsync(string url, string data)
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(900000)
            };
            HttpContent content = new StringContent(data);
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return response;
        }
        
        static async Task<HttpResponseMessage> GetAsync(string url)
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(900000)
            };
             HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return response;
        }
        
        #endregion
        
        
    }
}