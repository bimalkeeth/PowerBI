using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientCommon.Contract;
using PowerBIService.Common;

namespace PowerConsole
{
    class Program
    {
        public static readonly string TenantId = "470cec91-5a0e-47c7-87a9-2fcaf82d5d90";
        public static readonly string SecretId = "gvQTuhCBvCEcMbuw3L1QV6il9qluUvkJ6PL4hr8hxzE=";
        public static readonly string ApplicationId = "bcd57285-ddd1-4ae8-a8ad-cb72f7d24aaf";
        
        static void Main(string[] args)
        {
            //CreateGroup();
            //AddUserToGroup("f5057d7f-5f90-46fc-b5cc-97c5e1f218ac");
            //GetAllGroups();
            //GetAllReportInWorkSpace("463b0868-5933-4a89-9fa6-458d170aba31");
            AssignUsersToCloneReport("3d5434f2-4e87-4da0-af27-3058986d42f4","7567ccac-08ab-4cc4-ad55-476fd0b55acf");
        }


        public static void CreateGroup()
        {
            var service=new PowerBIService.Services.Implementation.PowerService();
            var dd= Task.Run(async () => await service.CreateGroup(new GroupCreateRequest
            {
                GroupName = "Predictor Reports",
                Credential = new UserData
                {
                    TenantId =TenantId ,
                    SecretId =  SecretId,
                    ApplicationId = ApplicationId
                },
                Members = new List<MembersRights>
                {
                    new MembersRights
                    {
                        MemberEmail = "bkaluarachchi@assetic.com",
                        GroupUserAccessRight = "Admin"
                    },
                    new MembersRights
                    {
                        MemberEmail = "jchang@assetic.com",
                        GroupUserAccessRight = "Admin"
                    }
                }.ToArray()
               
            })).ConfigureAwait(false);
            dd.GetAwaiter().GetResult(); 
            Console.WriteLine("Hello World!");
        }
        public static void AddUserToGroup(string GroupId)
        {
            var service=new PowerBIService.Services.Implementation.PowerService();
            var dd= Task.Run(async () => await service.AssignUsersToGroup(new GroupMemberAssignRequest
            {
                GroupId = GroupId,
                Credential = new UserData
                {
                    TenantId =TenantId ,
                    SecretId =  SecretId,
                    ApplicationId = ApplicationId
                },
                Members = new List<MembersRights>
                {
                    new MembersRights
                    {
                        MemberEmail = "jchang@assetic.com",
                        GroupUserAccessRight = "Admin"
                    }
                }.ToArray()
               
            })).ConfigureAwait(false);
            dd.GetAwaiter().GetResult(); 
            Console.WriteLine("Hello World!");
        }
        
        public static void GetAllGroups()
        {
            var service=new PowerBIService.Services.Implementation.PowerService();
            var dd= Task.Run(async () => await service.GetAllGroups(new UserData
            {
                TenantId =TenantId ,
                SecretId =  SecretId,
                ApplicationId = ApplicationId
            })).ConfigureAwait(false);
           var result= dd.GetAwaiter().GetResult();

           foreach (var group in result)
           {
              Console.WriteLine($"{group.Name}->{group.Id}");   
           }
           
           
            Console.WriteLine("Hello World!");
        }
        
        public static void GetAllReportInWorkSpace(string GroupId)
        {
            var service=new PowerBIService.Services.Implementation.PowerService();
            var dd= Task.Run(async () => await service.GetAllReportInWorkSpace(  new GetReportRequest
            {
                Credential = new UserData
                {
                   TenantId =TenantId ,
                   SecretId =  SecretId,
                   ApplicationId = ApplicationId
                },
                WorkSpaceId = GroupId

            }  )).ConfigureAwait(false);
            var result= dd.GetAwaiter().GetResult();

            foreach (var group in result)
            {
                Console.WriteLine($"{group.Name}->{group.Id}");   
            }
           
            Console.WriteLine("Hello World!");
        }
        
        
        public static void AssignUsersToCloneReport(string GroupId,string ReportId)
        {
            var service=new PowerBIService.Services.Implementation.PowerService();
            var dd= Task.Run(async () => await service.AddUsersToClonedReport(  new UserDataSetRequest
            {
                Credential = new UserData
                {
                    TenantId =TenantId ,
                    SecretId =  SecretId,
                    ApplicationId = ApplicationId
                },
                GroupId = GroupId,
                ReportId =ReportId 

            }  )).ConfigureAwait(false);
            var result= dd.GetAwaiter().GetResult();
          
           
            Console.WriteLine("Hello World!");
        }
    }
}