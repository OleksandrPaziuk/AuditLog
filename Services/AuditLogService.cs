using AuditLog.Enums;
using AuditLog.Models;
using System;
using System.Collections.Generic;

namespace AuditLog.Services
{
    public interface IAuditLogService
    {
        void AddRecord(bool isPlanned, TypeOfChange typeOfChange, string originalValue,
            string newValue, List<Approval> approvalList);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly IGlobalVariablesService _globalVariablesService = new GlobalVariablesService();

        public void AddRecord(bool isPlanned, TypeOfChange typeOfChange, string originalValue,
            string newValue, List<Approval> approvalList)
        {
            var auditLogEntry = new AuditLogEntry()
            {
                StartDateOfChange = _globalVariablesService.GetGlobalStartDateOfChange(),
                EndDateOfChange = DateTime.UtcNow,
                // AffectedDays // todo ask
                TypeOfChange = typeOfChange.ToString(),
                IsPlanned = isPlanned,
                OriginalValue = originalValue,
                NewValue = newValue,
                Approvals = approvalList //todo ask
            };

            _globalVariablesService.AddGlobalAuditLogEntry(auditLogEntry);
        }
    }
}