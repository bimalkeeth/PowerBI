using System;
using PowerBIService.Common;

namespace PowerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            
         var service=new PowerBIService.Services.Implementation.PowerService();

         service.EmbedReport(new UserData
         {
             PassWord = "Scala@1234",
             UserName = "bkaluarachchi@assetic.com",
             ApplicationId = "66bec1b2-4684-4a08-9f2b-b67216d4695a"
         });
         
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