using System;
using System.Linq;
using System.Threading.Tasks;
using ClientCommon.Contract;
using Microsoft.PowerBI.Api.V2;

namespace PowerBIService.Services.Implementation
{
    public partial class PowerService
    {
        public async Task<CloneReportResponse[]> CloneDashBoard(CloneReportRequest cloneReportRequest)
        {
            
            return new CloneReportResponse[] { };
        }
        public async Task<bool> AddUsersToClonedReport(UserDataSetRequest userDataSetRequest)
        {
            UserCredential = userDataSetRequest.Credential;
            await AuthenticateAsync();
            using (var pClient = new PowerBIClient(new Uri(POWER_BI_API_URL), PTokenCredentials))
            {
                var groupSearch =
                    await pClient.Groups.GetGroupsWithHttpMessagesAsync($"id eq '{userDataSetRequest.GroupId}'");
                if (!groupSearch.Body.Value.Any())
                    return false;

                var group = groupSearch.Body.Value.FirstOrDefault();
                if (group == null) return false;

                var tiles =await pClient.Dashboards.GetTilesInGroupWithHttpMessagesAsync(group.Id,"22b5dd77-8048-4237-887d-a91391f65414");

                var tile = tiles.Body?.Value.FirstOrDefault();
                if (tile == null)
                    return false;

                var datasets =await pClient.Datasets.GetDatasetsInGroupWithHttpMessagesAsync(group.Id);
                var dataset= datasets.Body.Value.FirstOrDefault(s => s.Id == tile.DatasetId);
               

            }
            return false;
        }
    }
}