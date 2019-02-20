using PowerBIService.Common;

namespace ClientCommon.Contract
{
    public class GetReportRequest
    {
        public string WorkSpaceId { get; set; }
        public UserData Credential { get; set; }
    }
}