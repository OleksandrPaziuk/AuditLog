using AuditLog.Enums;
using AuditLog.Models;
using System;

namespace AuditLog.Services
{
    public interface IValidationService
    {
        bool RouteValidation(Route originalFile, Route updatedFile);
    }

    public class ValidationService : IValidationService
    {
        private readonly IAuditLogService _auditLogService = new AuditLogService();
        private readonly IFileService _fileService = new FileService();
        private readonly IGlobalVariablesService _globalVariablesService = new GlobalVariablesService();

        public bool RouteValidation(Route originalFile, Route updatedFile)
        {
            _globalVariablesService.SetGlobalStartDateOfChange(DateTime.UtcNow);
            if (updatedFile.StartDate.AddYears(1) < updatedFile.EndDate)
            {
                _auditLogService.AddRecord(false, TypeOfChange.RouteNotValid, typeof(Passenger).Name, "", null);
                _fileService.WriteOutputFile();
                return false;
            }

            return true;
        }
    }
}