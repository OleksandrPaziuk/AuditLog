using AuditLog.Enums;
using AuditLog.Models;
using System.Collections.Generic;
using System.Linq;

namespace AuditLog.Services
{
    public interface IRouteProcessService
    {
        void ProcessRoute(Route originalFile, Route updatedFile);
    }

    public class RouteProcessService : IRouteProcessService
    {
        private readonly IAuditLogService _auditLogService = new AuditLogService();
        private readonly IComparisonService _comparisonService = new ComparisonService();

        public void ProcessRoute(Route originalFile, Route updatedFile)
        {
            ProcessRides(originalFile.Rides, updatedFile.Rides);
        }

        private void ProcessRides(IList<Ride> originalRideList, IList<Ride> updatedRideList)
        {
            foreach (var originalRide in originalRideList)
            {
                var updatedRide = updatedRideList.FirstOrDefault(x => x.DateRide == originalRide.DateRide);

                if (updatedRide == null)
                {
                    AddRecord(false, TypeOfChange.ObjectNotExist, typeof(Station).Name, "", null);
                    return;
                }

                // todo fix
                var approvalList = new List<Approval>()
                {
                    new Approval()
                    {
                        Driver = updatedRide.Driver
                    }
                };

                ProcessDriverObject(originalRide, updatedRide, approvalList);

                // todo fix
                approvalList = new List<Approval>()
                {
                    new Approval()
                    {
                        Driver = updatedRide.Driver
                    }
                };

                ProcessStartTime(originalRide, updatedRide, approvalList);

                ProcessStations(originalRide.Stations, updatedRide.Stations, approvalList);
            }
        }

        private void ProcessDriverObject(Ride originalRide, Ride updatedRide, List<Approval> approvalList)
        {
            if (!_comparisonService.ObjectComparison(originalRide.PlannedDriver, updatedRide.PlannedDriver))
            {
                AddRecord(true, TypeOfChange.ChangeDriver, "", "", approvalList); // todo
            }

            if (!_comparisonService.ObjectComparison(updatedRide.PlannedDriver, updatedRide.Driver))
            {
                approvalList.Add(new Approval()
                {
                    Driver = updatedRide.PlannedDriver,
                    Approved = false
                });

                AddRecord(false, TypeOfChange.ChangeDriver, "", "", approvalList);
            }
        }

        private void ProcessStartTime(Ride originalRide, Ride updatedRide, List<Approval> approvalList)
        {
            _comparisonService.ObjectComparison<Ride>(originalRide, updatedRide, nameof(originalRide.PlannedStartTime),
                nameof(originalRide.StartTime), TypeOfChange.ChangeStartTime, approvalList);
        }

        private void ProcessStations(IList<Station> originalStations, IList<Station> updatedStations,
            List<Approval> approvalList)
        {
            foreach (var originalStation in originalStations)
            {
                var updatedStation = updatedStations.FirstOrDefault(x =>
                    x.Name == originalStation.Name && x.Address == originalStation.Address);

                if (updatedStation == null)
                {
                    AddRecord(false, TypeOfChange.ObjectNotExist, typeof(Station).Name, "", null);
                    return;
                }

                _comparisonService.ObjectComparison<Station>(originalStation, updatedStation,
                    nameof(originalStation.PlannedOrder),
                    nameof(originalStation.Order), TypeOfChange.ChangeStation, approvalList);

                if (originalStation.IsActive != updatedStation.IsActive)
                {
                    AddRecord(false,
                        TypeOfChange.ChangeStationStatus,
                        originalStation.IsActive.ToString(), updatedStation.IsActive.ToString(), approvalList);
                }

                ProcessPassengers(originalStation, updatedStation, approvalList);
            }
        }

        private void ProcessPassengers(Station originalStation, Station updatedStationPassengers,
            List<Approval> approvalList)
        {
            foreach (var originalPassenger in originalStation.Passengers)
            {
                var updatedPassenger = updatedStationPassengers.Passengers.FirstOrDefault(x =>
                    x.FirstName == originalPassenger.FirstName &&
                    x.LastName == originalPassenger.LastName);

                if (updatedPassenger == null)
                {
                    AddRecord(false, TypeOfChange.ObjectNotExist, typeof(Passenger).Name, "", null);
                    return;
                }

                if ((originalPassenger.DestinationStation.Order < updatedPassenger.DestinationStation.Order) &&
                    updatedPassenger.IsActive)
                {
                    AddRecord(false, TypeOfChange.DestinationTooEarly,
                        originalPassenger.DestinationStation.Order.ToString(),
                        updatedPassenger.DestinationStation.Order.ToString(), approvalList);
                }

                if ((originalPassenger.DestinationStation.Order > updatedPassenger.DestinationStation.Order) &&
                    updatedPassenger.IsActive)
                {
                    AddRecord(false, TypeOfChange.DestinationTooLate,
                        originalPassenger.DestinationStation.Order.ToString(),
                        updatedPassenger.DestinationStation.Order.ToString(), approvalList);
                }
            }
        }

        private void AddRecord(bool isPlanned, TypeOfChange typeOfChange, string originalValue, string newValue,
            List<Approval> approvalList)
        {
            _auditLogService.AddRecord(isPlanned, typeOfChange, originalValue, newValue, approvalList);
        }
    }
}