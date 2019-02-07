using System.Threading.Tasks;
using Microsoft.Rest;
using PowerBIService.Common;
using PowerBIService.Services.Base;
using PowerBIService.Services.Interfaces;

namespace PowerBIService.Services.Implementation
{
    public class PowerService:PowerServiceBase,IPowerService
    {
        
        protected EmbedConfig         MEmbedConfig;
        protected TileEmbedConfig     MTileEmbedConfig;
        protected TokenCredentials    MTokenCredentials;


        public PowerService()
        {
            MTokenCredentials = null;
            MEmbedConfig = new EmbedConfig();
            MTileEmbedConfig = new TileEmbedConfig();
        }
        public EmbedConfig EmbedReport(UserData userData)
        {
            UserData = userData;
            var data = Task.Run(async () => await AuthenticateAsync()).ConfigureAwait(false);
           //var ss= Task.Run(async () => await GetAccessToken()).ConfigureAwait(false);
             data.GetAwaiter().GetResult();
            return null;
        }
        
       
    }
}