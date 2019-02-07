using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using PowerBIService.Common;

namespace PowerBIService.Services.Base
{
    public abstract class PowerServiceBase
    {
        #region Base Variables
        protected UserData UserData { get; set; }
        protected HttpClient RestClient { get; set; }
        protected string Token { get; set; }
        
        private static string POWER_BI_API_URL = "https://api.powerbi.com";
        private static string POWER_BI_AUTHORITY_URL = "https://login.windows.net/common/oauth2/authorize/";
        private static string POWER_BI_AUTHORITY_TOKEN_URL = "https://login.windows.net/common/oauth2/token";
        private static string POWER_BI_RESOURCE_URL = "https://analysis.windows.net/powerbi/api";

       
        
        #endregion
        protected PowerServiceBase()
        {
            RestClient=new HttpClient();
        }
        #region Authontication
        protected  async Task<OAuthResult> AuthenticateAsync()
        {
            var oauthEndpoint = new Uri(POWER_BI_AUTHORITY_URL);

            using (var client = new HttpClient())
            {
                var result = await client.PostAsync(oauthEndpoint, new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("resource", POWER_BI_RESOURCE_URL),
                    new KeyValuePair<string, string>("client_id", UserData.ApplicationId),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", UserData.UserName),
                    new KeyValuePair<string, string>("password", UserData.PassWord),
                    new KeyValuePair<string, string>("scope", "openid"),
                }));
                result.EnsureSuccessStatusCode();
                
                var content = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OAuthResult>(content);
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