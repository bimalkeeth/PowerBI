

using PowerBIService.Common;

namespace ClientCommon.Contract
{
    public class CloneReportRequest
    {
        public UserData Credential { get; set; }
        public CloneReport[] CloneReports { get; set; }
        public string ParentWorkSpace { get; set; }

        public string ClientWorkSpace { get; set; }
    }
}