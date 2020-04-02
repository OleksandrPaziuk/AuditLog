using AuditLog.Enums;
using AuditLog.Models;
using System.Collections.Generic;
using System.Linq;

namespace AuditLog.Services
{
    public interface IAnalyzeService
    {
        void AnalyzeJsonFiles(Route originalFile, Route updatedFile);
    }

    public class AnalyzeService : IAnalyzeService
    {
        private readonly IAuditLogService _auditLogService = new AuditLogService();
        private readonly IComparisonService _comparisonService = new ComparisonService();


        public void AnalyzeJsonFiles(Route originalFile, Route updatedFile)
        {
            AnalyzeRidesList(originalFile.Rides, updatedFile.Rides);
        }


        private void AnalyzeRidesList(IList<Ride> originalRideList, IList<Ride> updatedRideList)
        {
            foreach (var originalRide in originalRideList)
            {
                var updatedRide = updatedRideList.FirstOrDefault(x => x.DateRide == originalRide.DateRide);

                if (updatedRide == null)
                {
                    _auditLogService.GenerateAuditLog(TypeChange.Unplanned,
                        $"{typeof(Station).Name}{PotentialErrorType.ObjectNotExist}",
                        "", "", null);
                    return;
                }

                var approvalList = new List<Approval>()
                {
                    new Approval()
                    {
                        Driver = updatedRide.Driver
                    }
                };

                AnalyzeDriverObject(originalRide, updatedRide, approvalList);

                // todo fix
                approvalList = new List<Approval>()
                {
                    new Approval()
                    {
                        Driver = updatedRide.Driver
                    }
                };

                AnalyzeStartTimeValue(originalRide, updatedRide, approvalList);

                AnalyzeStationList(originalRide.Stations, updatedRide.Stations, approvalList);
            }
        }

        private void AnalyzeDriverObject(Ride originalRide, Ride updatedRide, List<Approval> approvalList)
        {
            if (!_comparisonService.ObjectComparison(originalRide.PlannedDriver, updatedRide.PlannedDriver))
            {
                _auditLogService.GenerateAuditLog(TypeChange.Planned, PotentialErrorType.ChangeDriver.ToString(),
                    originalRide.PlannedDriver.ToString(), updatedRide.PlannedDriver.ToString(), approvalList);
            }

            if (!_comparisonService.ObjectComparison(updatedRide.PlannedDriver, updatedRide.Driver))
            {
                approvalList.Add(new Approval()
                {
                    Driver = updatedRide.PlannedDriver,
                    Approved = false
                });

                _auditLogService.GenerateAuditLog(TypeChange.Unplanned, PotentialErrorType.ChangeDriver.ToString(),
                    updatedRide.PlannedDriver.ToString(), updatedRide.Driver.ToString(), approvalList);
            }
        }

        private void AnalyzeStartTimeValue(Ride originalRide, Ride updatedRide, List<Approval> approvalList)
        {
            _comparisonService.ObjectComparison<Ride>(originalRide, updatedRide, nameof(originalRide.PlannedStartTime),
                nameof(originalRide.StartTime), PotentialErrorType.ChangeStartTime, approvalList);
        }

        private void AnalyzeStationList(IList<Station> originalStations, IList<Station> updatedStations,
            List<Approval> approvalList)
        {
            foreach (var originalStation in originalStations)
            {
                var updatedStation = updatedStations.FirstOrDefault(x =>
                    x.Name == originalStation.Name && x.Address == originalStation.Address);

                if (updatedStation == null)
                {
                    _auditLogService.GenerateAuditLog(TypeChange.Unplanned,
                        $"{typeof(Station).Name}{PotentialErrorType.ObjectNotExist}",
                        "", "", null);
                    return;
                }

                _comparisonService.ObjectComparison<Station>(originalStation, updatedStation,
                    nameof(originalStation.PlannedOrder),
                    nameof(originalStation.Order), PotentialErrorType.ChangeStation, approvalList);

                if (updatedStation != null && originalStation.IsActive != updatedStation.IsActive)
                {
                    _auditLogService.GenerateAuditLog(TypeChange.Unplanned,
                        PotentialErrorType.ChangeStationStatus.ToString(),
                        originalStation.IsActive.ToString(), updatedStation.IsActive.ToString(), approvalList);
                }

                AnalyzePassengerList(originalStation, updatedStation, approvalList);
            }
        }

        private void AnalyzePassengerList(Station originalStation, Station updatedStationPassengers,
            List<Approval> approvalList)
        {
            foreach (var originalPassenger in originalStation.Passengers)
            {
                var updatedPassenger = updatedStationPassengers.Passengers.FirstOrDefault(x =>
                    x.FirstName == originalPassenger.FirstName &&
                    x.LastName == originalPassenger.LastName);

                if (updatedPassenger == null)
                {
                    _auditLogService.GenerateAuditLog(TypeChange.Unplanned,
                        $"{typeof(Passenger).Name}{PotentialErrorType.ObjectNotExist}", "", "", null);
                    return;
                }

                if ((originalPassenger.DestinationStation.Order < updatedPassenger.DestinationStation.Order) &&
                    updatedPassenger.IsActive)
                {
                    _auditLogService.GenerateAuditLog(TypeChange.Unplanned,
                        $"{typeof(Passenger).Name}{PotentialErrorType.DestinationTooEarly}",
                        originalPassenger.DestinationStation.Order.ToString(),
                        updatedPassenger.DestinationStation.Order.ToString(), approvalList);
                }

                if ((originalPassenger.DestinationStation.Order > updatedPassenger.DestinationStation.Order) &&
                    updatedPassenger.IsActive)
                {
                    _auditLogService.GenerateAuditLog(TypeChange.Unplanned,
                        $"{typeof(Passenger).Name}{PotentialErrorType.DestinationTooLate}",
                        originalPassenger.DestinationStation.Order.ToString(),
                        updatedPassenger.DestinationStation.Order.ToString(), approvalList);
                }
            }
        }
    }
}