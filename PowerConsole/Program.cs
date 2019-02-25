using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientCommon.Contract;
using PowerBIService.Common;

namespace PowerConsole
{
    class Program
    {
        public static readonly string TenantId = "";
        public static readonly string SecretId = "";
        public static readonly string ApplicationId = "";
        
        static void Main(string[] args)
        {
          
           
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