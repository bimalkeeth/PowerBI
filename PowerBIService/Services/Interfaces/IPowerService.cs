using System.Threading.Tasks;
using ClientCommon.Contract;
using PowerBIService.Common;

namespace PowerBIService.Services.Interfaces
{
    public interface IPowerService
    {
        /// <summary>
        /// Embed Report
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        EmbedConfig EmbedReport(UserData userData);
        
        /// <summary>
        /// Create Group
        /// </summary>
        /// <param name="groupCreateRequest"></param>
        /// <returns></returns>
        Task<bool> CreateGroup(GroupCreateRequest groupCreateRequest);

        /// <summary>
        /// Clone one or more reports
        /// </summary>
        /// <param name="cloneReportRequest"></param>
        /// <returns></returns>
        Task<CloneReportResponse[]> CloneReports(CloneReportRequest cloneReportRequest);
    }

   
}