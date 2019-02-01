

using System;
using Microsoft.PowerBI.Api.V1;
using Microsoft.Rest;
using PowerBIService.Common;

namespace PowerBIService
{
    public class PowerService:IPowerService
    {
        
        protected EmbedConfig         m_embedConfig;
        protected TileEmbedConfig     m_tileEmbedConfig;
        protected TokenCredentials    m_tokenCredentials;


        public PowerService()
        {
            m_tokenCredentials = null;
            m_embedConfig = new EmbedConfig();
            m_tileEmbedConfig = new TileEmbedConfig();
        }
        
        public void Connect()
        {
            using (var client = new PowerBIClient(new Uri("ApiUrl"), m_tokenCredentials))
            {
                
            }
        }
    }
}