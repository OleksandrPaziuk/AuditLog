using AuditLog.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace AuditLog.Services
{
    public interface IFileService
    {
        void ReadInputFiles(ref Route updatedFile, ref Route originalFile);
        void WriteOutputFile();
    }

    public class FileService : IFileService
    {
        private readonly IGlobalVariablesService _globalVariablesService = new GlobalVariablesService();

        public void ReadInputFiles(ref Route updatedFile, ref Route originalFile)
        {
            using (StreamReader r =
                new StreamReader(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"..\..\..\Input\Original.json"))))
            {
                originalFile = JsonConvert.DeserializeObject<Route>(r.ReadToEnd());
            }

            using (StreamReader r =
                new StreamReader(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"..\..\..\Input\Updated.json"))))
            {
                updatedFile = JsonConvert.DeserializeObject<Route>(r.ReadToEnd());
            }
        }

        public void WriteOutputFile()
        {
            string json = JsonConvert.SerializeObject(_globalVariablesService.GetGlobalAuditLogEntry().ToArray(),
                Formatting.Indented);

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Output\Output.json"),
                json);
        }
    }
}