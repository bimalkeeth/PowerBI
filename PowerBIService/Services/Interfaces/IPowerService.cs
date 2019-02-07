using PowerBIService.Common;

namespace PowerBIService.Services.Interfaces
{
    public interface IPowerService
    {
        EmbedConfig EmbedReport(UserData userData);
    }
}