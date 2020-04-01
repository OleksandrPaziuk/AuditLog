using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AuditLog.Models;
using Newtonsoft.Json;

namespace AuditLog.Services
{
    public interface IReadWriteService
    {
        void ReadFile();
    }
    public class ReadWriteService
    {
        public void ReadFile()
        {
            using (StreamReader r = new StreamReader(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\input\update.json"))))
            {
                string json = r.ReadToEnd();
                Route items = JsonConvert.DeserializeObject<Route>(json);
            }
        }

        public void WriteFile()
        {
            //todo
        }
    }
}