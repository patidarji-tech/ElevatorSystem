using ElevatorSystem.Abstraction;
using ElevatorSystem.ElevatorImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem
{
    /// <summary>
    /// Manages the operation and scheduling of a fleet of elevator cars within a building.
    /// </summary>
    /// <remarks>The <see cref="ControlRoom"/> class coordinates multiple elevator cars, handling passenger
    /// calls and scheduling elevators to efficiently service requests. It operates asynchronously, simulating elevator
    /// operations and generating random passenger calls. The class subscribes to elevator events to manage passenger
    /// boarding and destination requests.</remarks>
    public class ControlRoom
    {
        private readonly List<IElevatorCar> _elevators;
        private readonly IElevatorScheduler _scheduler;
        private readonly IClock _clock;
        private readonly ILogger _logger;
        private readonly Random _random = new Random();
        private const int TotalFloors = 10;
        // Simple mapping of waiting passengers' chosen destination until they board
        private readonly object _lock = new object();


        public ControlRoom(IEnumerable<IElevatorCar> elevators, IElevatorScheduler scheduler, IClock clock, ILogger logger)
        {
            _elevators = new List<IElevatorCar>(elevators);
            _scheduler = scheduler;
            _clock = clock;
            _logger = logger;

            // Subscribe to FloorArrived events 
            foreach (var e in _elevators)
            {
                if (e is ElevatorCar ec)
                {
                    ec.FloorArrived += OnFloorArrived;
                }
            }
        }

        /// <summary>
        /// Initiates the asynchronous operation of all elevators and generates random calls.
        /// </summary>
        /// <remarks>This method concurrently starts the operation of each elevator and generates random
        /// calls to simulate elevator requests. It waits for all operations to complete before returning.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StartAsync()
        {
            var elevatorTasks = _elevators.Select(e => e.OperateAsync());
            var callTask = GenerateRandomCallsAsync();
            await Task.WhenAll(elevatorTasks.Concat(new[] { callTask }));
        }


        /// <summary>
        /// Continuously generates random elevator calls, assigning them to available elevators.
        /// </summary>
        /// <remarks>This method simulates passengers requesting elevator service by randomly selecting a
        /// pickup floor and a destination floor. It logs each new call and assigns it to an elevator using the
        /// scheduler. The method runs indefinitely, introducing a delay between calls to simulate real-world call
        /// frequency.</remarks>
        /// <returns></returns>
        private async Task GenerateRandomCallsAsync()
        {
            while (true)
            {
                int callFloor = _random.Next(1, TotalFloors + 1);
                //int destinationFloor = _random.Next(1, TotalFloors + 1);
                //while (destinationFloor == callFloor) destinationFloor = _random.Next(1, TotalFloors + 1);

                _logger.Log($"\n*** New Call: Passenger at Floor {callFloor}  ***");
                // Only assign the pickup floor; the destination will be added when the elevator arrives
                _scheduler.AssignCall(_elevators, callFloor);

                await _clock.DelayAsync(_random.Next(120, 180)); // Use abstracted clock for call frequency
            }
        }

      

        /// <summary>
        /// Handles the event when an elevator car arrives at a specified floor.
        /// </summary>
        /// <remarks>This method assigns a random destination floor for the elevator car, ensuring it is
        /// different from the current floor. It logs the event of a passenger boarding the elevator and requesting a
        /// destination.</remarks>
        /// <param name="car">The elevator car that has arrived at the floor.</param>
        /// <param name="floor">The floor number where the elevator car has arrived.</param>
        private void OnFloorArrived(Abstraction.IElevatorCar car, int floor)
        {
            int destination = -1;
            lock (_lock)
            {
                int destinationFloor = _random.Next(1, TotalFloors + 1);
                while (destinationFloor == floor) destinationFloor = _random.Next(1, TotalFloors + 1);

                destination = destinationFloor;

            }

            if (destination != -1)
            {
                _logger.Log($"ControlRoom: Passenger boarded Elevator {car.Id} at floor {floor}, requesting destination {destination}.");
                car.AddDestination(destination);
            }
            else
            {
                // No pending passenger for this floor in the simple demo
                _logger.Log($"ControlRoom: Elevator {car.Id} arrived at floor {floor} but no waiting passenger recorded.");
            }
        }
    }
}
