namespace PowerBIService.Common
{
    public class UserData
    {
        
        /// <summary>
        /// Power BI Azure Authorized Application ID
        /// </summary>
        public string ApplicationId { get; set; }
        /// <summary>
        /// App Workspace ID
        /// </summary>
        public string WorkSpaceId { get; set; }
        /// <summary>
        /// Report Name
        /// </summary>
        public string ReportId { get; set; }
        
        public string EmbedUrlBase { get; set; }
        
        public string UserName { get; set; }
        public string PassWord { get; set; }
        
    }
}