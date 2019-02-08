using System.Threading.Tasks;
using ClientCommon.Contract;
using PowerBIService.Common;

namespace PowerBIService.Services.Interfaces
{
    public interface IPowerService
    {
        EmbedConfig EmbedReport(UserData userData);
        Task<bool> CreateGroup(GroupCreateRequest groupCreateRequest);
    }

   
}