using System;
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
        private readonly IGlobalVariablesService _globalVariablesService = new GlobalVariablesService();

        public void ProcessRoute(Route originalFile, Route updatedFile)
        {
            ProcessRides(originalFile.Rides, updatedFile.Rides);
            ProcessAffectedDays(updatedFile);
        }

        private void ProcessRides(IList<Ride> originalRideList, IList<Ride> updatedRideList)
        {
            foreach (var originalRide in originalRideList)
            {
                var updatedRide = updatedRideList.FirstOrDefault(x => x.DateRide == originalRide.DateRide);

                if (updatedRide == null)
                {
                    AddRecord(false, TypeOfChange.ObjectNotExist, typeof(Station).Name, "", null, updatedRide.DateRide);
                    return;
                }

                var approvalList = new List<Approval>()
                {
                    new Approval()
                    {
                        Driver = updatedRide.Driver
                    }
                };

                approvalList = ProcessDriverObject(originalRide, updatedRide, approvalList);

                ProcessStartTime(originalRide, updatedRide, approvalList);

                ProcessStations(originalRide, updatedRide, approvalList);
                //ProcessStations(originalRide.Stations, updatedRide.Stations, approvalList);
            }
        }

        private List<Approval> ProcessDriverObject(Ride originalRide, Ride updatedRide, List<Approval> approvalList)
        {
            //if (!_comparisonService.ObjectComparison(originalRide.PlannedDriver, updatedRide.PlannedDriver))
            //{
            //    AddRecord(true, TypeOfChange.ChangeDriver, "", "", approvalList); // todo
            //}

            if (!_comparisonService.ObjectComparison(originalRide.Driver, updatedRide.Driver))
            {
                approvalList.Add(new Approval()
                {
                    Driver = updatedRide.PlannedDriver,
                    Approved = false
                });

                AddRecord(false, TypeOfChange.ChangeDriver, "", "", approvalList, updatedRide.DateRide);

                //todo if need return onlu astive driver uncomment
                //return new List<Approval>()
                //{
                //    approvalList.First()
                //};
            }
            return approvalList;
        }

        private void ProcessStartTime(Ride originalRide, Ride updatedRide, List<Approval> approvalList)
        {
            _comparisonService.ObjectComparison<Ride>(originalRide, updatedRide, nameof(originalRide.PlannedStartTime),
                nameof(originalRide.StartTime), TypeOfChange.ChangeStartTime, approvalList, updatedRide.DateRide);
        }

        private void ProcessStations(Ride originalRide, Ride updatedRide,
            List<Approval> approvalList)
        {
            foreach (var originalStation in originalRide.Stations)
            {
                var updatedStation = updatedRide.Stations.FirstOrDefault(x =>
                    x.Name == originalStation.Name && x.Address == originalStation.Address);

                if (updatedStation == null)
                {
                    AddRecord(false, TypeOfChange.ObjectNotExist, typeof(Station).Name, "", null, updatedRide.DateRide);
                    return;
                }

                _comparisonService.ObjectComparison<Station>(originalStation, updatedStation,
                    nameof(originalStation.PlannedOrder),
                    nameof(originalStation.Order), TypeOfChange.ChangeStation, approvalList, updatedRide.DateRide);

                if (originalStation.IsActive != updatedStation.IsActive)
                {
                    AddRecord(false,
                        TypeOfChange.ChangeStationStatus,
                        originalStation.IsActive.ToString(), updatedStation.IsActive.ToString(), approvalList, updatedRide.DateRide);
                }

                ProcessPassengers(originalStation, updatedStation, approvalList, updatedRide.DateRide);
            }
        }

        private void ProcessPassengers(Station originalStation, Station updatedStationPassengers,
            List<Approval> approvalList, DateTime dateRide)
        {
            foreach (var originalPassenger in originalStation.Passengers)
            {
                var updatedPassenger = updatedStationPassengers.Passengers.FirstOrDefault(x =>
                    x.FirstName == originalPassenger.FirstName &&
                    x.LastName == originalPassenger.LastName);

                if (updatedPassenger == null)
                {
                    AddRecord(false, TypeOfChange.ObjectNotExist, typeof(Passenger).Name, "", null, dateRide);
                    return;
                }

                if ((originalPassenger.DestinationStation.Order < updatedPassenger.DestinationStation.Order) &&
                    updatedPassenger.IsActive)
                {
                    AddRecord(false, TypeOfChange.DestinationTooEarly,
                        originalPassenger.DestinationStation.Order.ToString(),
                        updatedPassenger.DestinationStation.Order.ToString(), approvalList, dateRide);
                }

                if ((originalPassenger.DestinationStation.Order > updatedPassenger.DestinationStation.Order) &&
                    updatedPassenger.IsActive)
                {
                    AddRecord(false, TypeOfChange.DestinationTooLate,
                        originalPassenger.DestinationStation.Order.ToString(),
                        updatedPassenger.DestinationStation.Order.ToString(), approvalList, dateRide);
                }
            }
        }

        private void AddRecord(bool isPlanned, TypeOfChange typeOfChange, string originalValue, string newValue,
            List<Approval> approvalList, DateTime dateOfChang, List<string> affectedDays = default)
        {
            _auditLogService.AddRecord(isPlanned, typeOfChange, originalValue, newValue, approvalList, dateOfChang, affectedDays);
        }

        private void ProcessAffectedDays(Route updatedRide)
        {
            var datesOfChange = _globalVariablesService.GetGlobalDatesOfChange();

            var affectedDays = new List<string>();

            foreach (var dateOfChange in datesOfChange)
            {
                var dateOfWeek = dateOfChange.DayOfWeek;
                if (updatedRide.ActiveDays[(int)dateOfWeek])
                {
                    affectedDays.Add(dateOfWeek.ToString());
                }
            }

            AddRecord(false, TypeOfChange.AffectedDays, "", "", null, default, affectedDays);

        }
    }
}