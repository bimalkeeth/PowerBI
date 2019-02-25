namespace WebClientDemo.Models
{
    public class GroupMemberAssignRequestVM
    {
        public string GroupId { get; set; }
        public CredentialVM Credential { get; set; }
        public MembersRightsVM[] Members { get; set; }
    }
}