namespace WebClientDemo.Models
{
    public class GroupCreateRequestVM
    {
        public string GroupName { get; set; }
        public CredentialVM Credential { get; set; }
        public MembersRightsVM[] Members { get; set; }
    }
}