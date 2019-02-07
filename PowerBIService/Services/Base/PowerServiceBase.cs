using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Easy_Http;
using Easy_Http.Builders;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerBIService.Common;

namespace PowerBIService.Services.Base
{

    public class AuthData
    {
        public string resource { get; set; }
        public string client_id { get; set; }
        public string grant_type { get; set; }
        public string username { get; set; }
        
        public string password { get; set; }
        
        public string scope { get; set; }
        
    }
    
    
    public abstract class PowerServiceBase
    {
        #region Base Variables
        protected UserData UserData { get; set; }
        protected HttpClient RestClient { get; set; }
        protected string Token { get; set; }
        
        private static string POWER_BI_API_URL = "https://api.powerbi.com";
        private static string POWER_BI_AUTHORITY_URL = "https://login.windows.net/common/oauth2/authorize/";
        private static string POWER_BI_AUTHORITY_TOKEN_URL = "https://login.windows.net/470cec91-5a0e-47c7-87a9-2fcaf82d5d90/oauth2/token";
        private static string POWER_BI_RESOURCE_URL = "https://analysis.windows.net/powerbi/api";

       
        
        #endregion
        protected PowerServiceBase()
        {
           // RestClient=new HttpClient();
        }
        #region Authontication
        protected  async Task<OAuthResult> AuthenticateAsync()
        {
            var oauthEndpoint = new Uri(POWER_BI_AUTHORITY_TOKEN_URL);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                
                
                
               var dd= await new RequestBuilder<AuthData>()
                    .SetHost(POWER_BI_AUTHORITY_TOKEN_URL)
                    .SetContentType(ContentType.Application_Json)
                    .SetType(RequestType.Post)
                    .SetModelToSerialize(new AuthData
                    {
                        resource = POWER_BI_RESOURCE_URL,
                        client_id = UserData.ApplicationId,
                        //grant_type = "password",
                        //username = UserData.UserName,
                       // password = UserData.PassWord,
                        scope = "read"
                    })
                    .Build()
                    .Execute();
                
                
                
                var result = await client.PostAsync(oauthEndpoint, new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("resource", POWER_BI_RESOURCE_URL),
                    new KeyValuePair<string, string>("client_id", UserData.ApplicationId),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", UserData.UserName),
                    new KeyValuePair<string, string>("password", UserData.PassWord),
                    //new KeyValuePair<string, string>("scope", "read")
                }));
                result.EnsureSuccessStatusCode();
                
                var content = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OAuthResult>(content);
            }
        }
        
        protected async Task<TokenCredentials> GetAccessToken()
        {
            using (HttpClient client = new HttpClient())
            {
                string tenantId = "";
                var tokenEndpoint =POWER_BI_AUTHORITY_TOKEN_URL;
                var accept = "application/json";
                var userName = "bkaluarachchi@assetic.com";
                var password = "Scala@1234";
                var clientId = "66bec1b2-4684-4a08-9f2b-b67216d4695a";

                client.DefaultRequestHeaders.Add("Accept", accept);
                string postBody = null;

                postBody = $@"resource="+POWER_BI_RESOURCE_URL+$@"
                        &client_id={clientId}
                        &grant_type=password
                        &username={userName}
                        &password={password}
                        &scope=openid";

                var tokenResult = await client.PostAsync(tokenEndpoint, new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded"));
                tokenResult.EnsureSuccessStatusCode();
                var tokenData = await tokenResult.Content.ReadAsStringAsync();

                JObject parsedTokenData = JObject.Parse(tokenData);

                var token = parsedTokenData["access_token"].Value<string>();
                return new TokenCredentials(token, "Bearer");
            }
        }
        
        
        
        protected  void GetAccessTokenSilently()
        {
            var request = WebRequest.CreateHttp(POWER_BI_AUTHORITY_TOKEN_URL);
            
            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentLength = 0;
            request.ContentType = "application/x-www-form-urlencoded";

            var parsedQueryString = HttpUtility.ParseQueryString(String.Empty);
            parsedQueryString.Add("client_id", UserData.ApplicationId);
            parsedQueryString.Add("grant_type", "password");
            parsedQueryString.Add("resource", POWER_BI_RESOURCE_URL);
            parsedQueryString.Add("username", UserData.UserName);
            parsedQueryString.Add("password", UserData.PassWord);
            
            var postData = parsedQueryString.ToString();
            
            var dataByteArray = System.Text.Encoding.ASCII.GetBytes(postData); ;
            request.ContentLength = dataByteArray.Length;

            try
            {
                //Write JSON byte[] into a Stream
                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(dataByteArray, 0, dataByteArray.Length);
                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var responseJson = JsonConvert.DeserializeObject<dynamic>(responseString);
                    Token =  responseJson["access_token"];
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string errorString = reader.ReadToEnd();
                            dynamic respJson = JsonConvert.DeserializeObject<dynamic>(errorString);
                            Console.WriteLine(respJson);
                            
                        }
                    }
                }
            } 

        }
        #endregion
        
        
    }
}