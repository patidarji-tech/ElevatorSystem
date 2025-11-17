using ElevatorSystem.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.ElevatorImplementation
{

    /// <summary>
    /// Provides a simple scheduling mechanism for assigning elevator calls to the most suitable elevator car.
    /// </summary>
    /// <remarks>This scheduler selects the elevator with the fewest current destinations, and in case of a
    /// tie,  the one closest to the call floor. It assigns the call floor to the selected elevator,  logging the
    /// assignment process.</remarks>
    public class SimpleElevatorScheduler : IElevatorScheduler
    {
        private readonly ILogger _logger;

        public SimpleElevatorScheduler(ILogger logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Assigns a call to the most suitable elevator from the available elevators based on current destinations and
        /// nearest to the call floor.
        /// </summary>
        /// <remarks>The method selects the elevator with the fewest current destinations and closest
        /// proximity to the call floor. It assigns the call floor as a destination for the selected elevator. The
        /// passenger's final destination should be added after the elevator arrives at the call floor.</remarks>
        /// <param name="elevators">A collection of elevators to consider for the call assignment. Each elevator must implement the 
        /// "IElevatorCar" interface.</param>
        /// <param name="callFloor">The floor from which the call originates. This is the floor where the elevator is requested to stop.</param>
        public void AssignCall(IEnumerable<IElevatorCar> elevators, int callFloor)
        {
            var bestElevator = elevators
                .OrderBy(e => e.Destinations.Count)
                .ThenBy(e => Math.Abs(e.CurrentFloor - callFloor))
                .First();

            //  assigning the call floor. The passenger's destination should be
            // added after the elevator arrives at the call floor (when passengers board).
            bestElevator.AddDestination(callFloor);
            _logger.Log($"Call assigned to Elevator {bestElevator.Id} by SimpleScheduler. Call floor: {callFloor}");
        }
    }

}
