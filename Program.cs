using AuditLog.Models;
using AuditLog.Services;
using Newtonsoft.Json;
using System;

namespace AuditLog
{
    static class Program
    {
        private static readonly IFileService FileService = new FileService();
        private static readonly IValidationService ValidationService = new ValidationService();
        private static readonly IRouteProcessService RouteProcessService = new RouteProcessService();
        private static readonly IGlobalVariablesService GlobalVariablesService = new GlobalVariablesService();

        static void Main()
        {
            //todo added open dialog window with select input file
            Route updatedFile = default, originalFile = default;
            FileService.ReadInputFiles(ref updatedFile, ref originalFile);

            if (!ValidationService.RouteValidation(originalFile, updatedFile))
            {
                Console.WriteLine("Rout does not valid");
            }

            RouteProcessService.ProcessRoute(originalFile, updatedFile);
            FileService.WriteOutputFile();

            //todo add grouping by params (now default group by isPlanned)

            Console.WriteLine(JsonConvert.SerializeObject(GlobalVariablesService.GetGlobalAuditLogEntry(),
                Formatting.Indented));

            Console.ReadKey();
        }
    }
}