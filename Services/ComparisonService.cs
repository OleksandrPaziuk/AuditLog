using System;
using AuditLog.Enums;
using AuditLog.Models;
using System.Collections.Generic;

namespace AuditLog.Services
{
    public interface IComparisonService
    {
        void ObjectComparison<T>(T originalObject, T updatedObject, string firstProperty,
            string secondProperty, TypeOfChange typeOfChange, List<Approval> approvalList, DateTime dateOdChange);

        bool ObjectComparison<T>(T object1, T object2);
    }

    public class ComparisonService : IComparisonService
    {
        private readonly IAuditLogService _auditLogService = new AuditLogService();

        public void ObjectComparison<T>(T originalObject, T updatedObject, string firstProperty,
            string secondProperty, TypeOfChange typeOfChange, List<Approval> approvalList, DateTime dateOdChange)
        {
            var originalFirstValue = typeof(T).GetProperty(firstProperty)?.GetValue(originalObject);
            var updatedFirstValue = typeof(T).GetProperty(firstProperty)?.GetValue(updatedObject);
            var updatedSecondValue = typeof(T).GetProperty(secondProperty)?.GetValue(updatedObject);

            if (!originalFirstValue.Equals(updatedFirstValue))
            {
                _auditLogService.AddRecord(true, typeOfChange, originalFirstValue.ToString(),
                    updatedFirstValue.ToString(), approvalList, dateOdChange);
            }

            if (!updatedFirstValue.Equals(updatedSecondValue))
            {
                _auditLogService.AddRecord(false, typeOfChange, updatedFirstValue.ToString(),
                    updatedSecondValue.ToString(), approvalList, dateOdChange);
            }
        }

        public bool ObjectComparison<T>(T object1, T object2)
        {
            foreach (var propertyInfo in typeof(Driver).GetProperties())
            {
                if (propertyInfo.GetValue(object1).ToString() != propertyInfo.GetValue(object2).ToString())
                {
                    return false;
                }
            }

            return true;
        }
    }
}