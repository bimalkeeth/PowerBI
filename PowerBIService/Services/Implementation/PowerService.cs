using Microsoft.Rest;
using PowerBIService.Common;

namespace PowerBIService.Services.Implementation
{
    public partial class PowerService
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
    }
}