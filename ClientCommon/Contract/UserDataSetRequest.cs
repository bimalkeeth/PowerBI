using PowerBIService.Common;

namespace ClientCommon.Contract
{
    public class UserDataSetRequest
    {
        public UserData Credential { get; set; }
        public string ReportId { get; set; }
        public string GroupId { get; set; }
        public ReportUsers[] Users { get; set; }
    }
}