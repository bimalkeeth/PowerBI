using System.Threading.Tasks;
using ClientCommon.Contract;
using PowerBIService.Common;

namespace PowerBIService.Services.Interfaces
{
    public interface IPowerService
    {
       
       
        Task<bool> CreateGroup(GroupCreateRequest groupCreateRequest);
       
        Task<CloneReportResponse[]> CloneReports(CloneReportRequest cloneReportRequest);

        Task<EmbedConfig> ClientEmbedReport(EmbedReportRequest embedReportRequest);

        Task<NameValueContract[]> GetAllGroups(UserData credential);

        Task<NameValueContract[]> GetAllReportInWorkSpace(GetReportRequest getReportRequest);

        Task<bool> AssignUsersToGroup(GroupMemberAssignRequest memberAssignRequest);
    }

   
}