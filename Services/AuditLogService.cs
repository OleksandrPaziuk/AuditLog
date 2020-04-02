using AuditLog.Enums;
using AuditLog.Models;
using System;
using System.Collections.Generic;

namespace AuditLog.Services
{
    public interface IAuditLogService
    {
        void GenerateAuditLog(TypeChange typeChange, string description, string originalValue,
            string newValue, List<Approval> approvalList);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly IGlobalVariablesService _globalVariablesService = new GlobalVariablesService();

        public void GenerateAuditLog(TypeChange typeChange, string description, string originalValue,
            string newValue, List<Approval> approvalList)
        {
            var auditLogEntry = new AuditLogEntry()
            {
                StartDateOfChange = _globalVariablesService.GetGlobalStartDateOfChange(),
                EndDateOfChange = DateTime.UtcNow,
                // AffectedDays // todo ask
                TypeChange = typeChange.ToString(),
                Description = description,
                OriginalValue = originalValue,
                NewValue = newValue,
                Approvals = approvalList //todo ask
            };

            _globalVariablesService.AddGlobalAuditLogEntry(auditLogEntry);
        }
    }
}