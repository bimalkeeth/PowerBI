

using PowerBIService.Common;

namespace ClientCommon.Contract
{
    public class GroupMemberAssignRequest
    {
        public string GroupId { get; set; }
        public UserData Credential { get; set; }
        
        public MembersRights[] Members { get; set; }
    }
}