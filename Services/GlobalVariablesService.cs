using AuditLog.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuditLog.Services
{
    public interface IGlobalVariablesService
    {
        DateTime GetGlobalStartDateOfChange();
        void SetGlobalStartDateOfChange(DateTime newStartDateOfChange);
        List<IGrouping<string, AuditLogEntry>> GetGlobalAuditLogEntry();
        void SetGlobalAuditLogEntry(List<AuditLogEntry> newAuditLogEntry);
        void AddGlobalAuditLogEntry(AuditLogEntry newAuditLogEntry);
    }

    public class GlobalVariablesService : IGlobalVariablesService
    {
        private static List<AuditLogEntry> AuditLogEntry { get; set; } = new List<AuditLogEntry>();
        private static DateTime StartDateOfChange { get; set; }

        public DateTime GetGlobalStartDateOfChange()
        {
            return StartDateOfChange;
        }

        public void SetGlobalStartDateOfChange(DateTime newStartDateOfChange)
        {
            StartDateOfChange = newStartDateOfChange;
        }

        public List<IGrouping<string, AuditLogEntry>> GetGlobalAuditLogEntry()
        {
            return AuditLogEntry.GroupBy(x => x.TypeOfChange).ToList();
        }

        public void SetGlobalAuditLogEntry(List<AuditLogEntry> newAuditLogEntry)
        {
            AuditLogEntry = newAuditLogEntry;
        }

        public void AddGlobalAuditLogEntry(AuditLogEntry newAuditLogEntry)
        {
            AuditLogEntry.Add(newAuditLogEntry);
        }
    }
}