using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientCommon.Contract;
using PowerBIService.Common;

namespace PowerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            
         var service=new PowerBIService.Services.Implementation.PowerService();

//         service.EmbedReport(new UserData
//         {
//             TenantId = "470cec91-5a0e-47c7-87a9-2fcaf82d5d90",
//             SecretId =  "ALYbNqinTaY8IU+q4uxWYgvGSdbBFyHeVJqfx1mb910=",
//             ApplicationId = "64f6409f-9683-42b5-9949-77a91767838e"
//         });



           var dd= Task.Run(async () => await service.CreateGroup(new GroupCreateRequest
           {
               GroupName = "CumberLand Reports",
               Credential = new UserData
               {
                   TenantId = "470cec91-5a0e-47c7-87a9-2fcaf82d5d90",
                   SecretId =  "gvQTuhCBvCEcMbuw3L1QV6il9qluUvkJ6PL4hr8hxzE=",
                   ApplicationId ="bcd57285-ddd1-4ae8-a8ad-cb72f7d24aaf"  //""75c13de1-9664-4445-84d8-73db0afc371f"
               },
               Members = new List<MembersRights>
               {
                   new MembersRights
                   {
                       MemberEmail = "bkaluarachchi@assetic.com",
                       GroupUserAccessRight = "Admin"
                   }
               }.ToArray()
               
           })).ConfigureAwait(false);
           dd.GetAwaiter().GetResult(); 
         
//           service.EmbedReport(new UserData
//           {
//               ApiUrl = "https://api.powerbi.com/",
//               ReportId ="" 
//               
//           });
//           
           
           
            Console.WriteLine("Hello World!");
        }
    }
}