using System;
using System.Linq;
using System.Threading.Tasks;
using Bifrost.Extensions;
using ClientCommon.Contract;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
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
            base.UserData = userData;
            var data = Task.Run(async () => await AuthenticateAsync()).ConfigureAwait(false);
            data.GetAwaiter().GetResult();
            return null;
        }

        
        public async Task<bool> CreateGroup(GroupCreateRequest groupCreateRequest)
        {
            base.UserData = groupCreateRequest.Credential;
            await AuthenticateAsync();
            using (var pClient = new PowerBIClient(new Uri(POWER_BI_API_URL), PTokenCredentials))
            {
               var group= await pClient.Groups.CreateGroupWithHttpMessagesAsync(new GroupCreationRequest{Name =groupCreateRequest.GroupName});

               if (groupCreateRequest.Members.Any())
               {
                   groupCreateRequest.Members.ForEach(async s =>
                   {
                       await pClient.Groups.AddGroupUserWithHttpMessagesAsync(group.Body.Id,
                           new GroupUserAccessRight
                               {EmailAddress = s.MemberEmail, GroupUserAccessRightProperty = s.GroupUserAccessRight}); 
                   });
               }
            }
            return false;
        }  
       
    }
}