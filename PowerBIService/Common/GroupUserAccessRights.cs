using System;

namespace PowerBIService.Common
{
    public static class GroupUserAccessRights
    {
        public const string None = "None";
        public const string Member = "Member";
        public const string Admin = "Admin";
        public const string Contributor = "Contributor";

        public static string GetRight(this string strRight)
        {
            if (String.Equals(strRight, None, StringComparison.CurrentCultureIgnoreCase))
            {
                return None;
            }
            if (String.Equals(strRight, Member, StringComparison.CurrentCultureIgnoreCase))
            {
                return Member;
            }
            if (String.Equals(strRight, Admin, StringComparison.CurrentCultureIgnoreCase))
            {
                return Admin;
            }
            if (String.Equals(strRight, Contributor, StringComparison.CurrentCultureIgnoreCase))
            {
                return Contributor;
            }
            return null;
        }
    }
}