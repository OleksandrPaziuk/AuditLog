using AuditLog.Enums;
using AuditLog.Models;
using System;
using System.Collections.Generic;

namespace AuditLog.Services
{
    public interface IAuditLogService
    {
        void AddRecord(bool isPlanned, TypeOfChange typeOfChange, string originalValue,
            string newValue, List<Approval> approvalList, DateTime dateOfChang = default,
            List<string> affectedDays = default);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly IGlobalVariablesService _globalVariablesService = new GlobalVariablesService();

        public void AddRecord(bool isPlanned, TypeOfChange typeOfChange, string originalValue,
            string newValue, List<Approval> approvalList, DateTime dateOfChang = default,
            List<string> affectedDays = default)
        {
            var auditLogEntry = new AuditLogEntry()
            {
                //todo ask (the date must be from the date of the change)
                StartDateOfChange = _globalVariablesService.GetGlobalStartDateOfChange(),
                EndDateOfChange = DateTime.UtcNow,
                AffectedDays = affectedDays,
                TypeOfChange = typeOfChange.ToString(),
                IsPlanned = isPlanned,
                OriginalValue = originalValue,
                NewValue = newValue,
                Approvals = approvalList
            };

            _globalVariablesService.AddGlobalAuditLogEntry(auditLogEntry);
            _globalVariablesService.AddGlobalDatesOfChange(dateOfChang);
        }
    }
}