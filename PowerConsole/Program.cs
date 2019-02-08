using System;
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
               GroupName = "TestGroup",
               Credential = new UserData
               {
                   TenantId = "470cec91-5a0e-47c7-87a9-2fcaf82d5d90",
                   SecretId =  "ALYbNqinTaY8IU+q4uxWYgvGSdbBFyHeVJqfx1mb910=",
                   ApplicationId = "64f6409f-9683-42b5-9949-77a91767838e"
               }
               
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