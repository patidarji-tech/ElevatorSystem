using ElevatorSystem.Abstraction;
using ElevatorSystem.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.ElevatorImplementation
{
    /// <summary>
    /// Represents an elevator car that can move between floors and handle passenger requests.
    /// </summary>
    /// <remarks>The ElevatorCar class manages the movement of an elevator, including handling
    /// destination requests and notifying when it arrives at a floor. It operates asynchronously and can be stopped for
    /// testing purposes. The elevator's movement is simulated using an abstracted clock to allow for time-based
    /// operations.</remarks>
    public class ElevatorCar : Abstraction.IElevatorCar
    {
        public int Id { get; }   //  Elevator ID Unique for each elevator
        public int CurrentFloor { get; private set; } = 1;
        public Direction Direction { get; private set; } = Direction.Idle;
        private readonly HashSet<int> _destinations = new HashSet<int>();
        public IReadOnlyCollection<int> Destinations => _destinations;
        private readonly object _lock = new object();
        private readonly IClock _clock;
        private readonly ILogger _logger;
        private const int TimePerFloor = 10;
        private const int DoorTime = 10;
        private volatile bool _running = true; // allow tests to stop the loop

        // Event raised when the elevator stops at a floor (doors open)
        public event Action<Abstraction.IElevatorCar, int>? FloorArrived;

        public ElevatorCar(int id, IClock clock, ILogger logger)
        {
            Id = id;
            _clock = clock;
            _logger = logger;
        }

        // Adds a new destination floor to the elevator's list of stops.
        public void AddDestination(int floor)
        {
            lock (_lock) _destinations.Add(floor);
        }

        // Added so tests can stop the infinite operate loop.
        public void Stop() => _running = false;


        /// <summary>
        /// Operates the elevator asynchronously, managing its movement between floors based on current destinations.
        /// </summary>
        /// <remarks>The method continuously checks for destinations and moves the elevator accordingly.
        /// It handles the elevator's direction and stops at each destination to allow passengers to board or exit.
        /// The elevator's state is logged throughout its operation. The method will continue to operate as long as the
        /// elevator is running.</remarks>
        /// <returns></returns>
        public async Task OperateAsync()
        {
            while (_running)
            {
                bool hasDestinations;
                lock (_lock)
                {
                    hasDestinations = _destinations.Count > 0;
                    if (!hasDestinations)
                    {
                        Direction = Direction.Idle;
                    }
                }

                if (!hasDestinations)
                {
                    await _clock.DelayAsync(1); // Wait a short time
                    continue;
                }

                int nextDestination = GetNextDestination();

                if (nextDestination == CurrentFloor)
                {
                    Direction = Direction.Idle;
                }
                else
                {
                    Direction = (nextDestination > CurrentFloor) ? Direction.Up : Direction.Down;
                }

                // Move towards the next destination one floor at a time
                while (CurrentFloor != nextDestination && _running)
                {
                    _logger.Log($"Elevator {Id}: Moving {Direction} from floor {CurrentFloor} towards {nextDestination}");
                    await _clock.DelayAsync(TimePerFloor); // Use abstracted clock
                    lock (_lock)
                    {
                        CurrentFloor += (Direction == Direction.Up) ? 1 : -1;
                    }
                    _logger.Log($"Elevator {Id}: Arrived at floor {CurrentFloor}");
                }

                bool stoppedForPassengers = false;

                // Handle arrival at a stop
                lock (_lock)
                {
                    if (_destinations.Contains(CurrentFloor))
                    {
                        _logger.Log($"Elevator {Id}: Stopping at floor {CurrentFloor} for passengers. Doors open.");
                        // remove the pickup/stop now so the elevator's destination list reflects that it's stopped here
                        _destinations.Remove(CurrentFloor);
                        stoppedForPassengers = true;
                    }
                }

                if (stoppedForPassengers)
                {
                    // Notify subscribers (e.g., ControlRoom) that elevator has arrived and doors are open.
                    // Invoke outside of the lock to avoid potential deadlocks.
                    try
                    {
                        FloorArrived?.Invoke(this, CurrentFloor);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"Elevator {Id}: Exception in FloorArrived handler: {ex.Message}");
                    }
                }

                // Wait for doors to be open (do outside lock to avoid blocking other threads)
                await _clock.DelayAsync(DoorTime);
                if (stoppedForPassengers)
                {
                    _logger.Log($"Elevator {Id}: Doors close. Ready to move.");
                }
            }
        }


        /// <summary>
        /// Determines the next floor destination based on the current direction and scheduled stops.
        /// </summary>
        /// <remarks>This method evaluates the list of scheduled destinations and returns the next floor
        /// to visit. If the current direction is up or idle, it prioritizes stops above or at the current floor. If the
        /// current direction is down, it prioritizes stops below or at the current floor. If no further stops are
        /// scheduled in the current direction, it switches to the opposite direction.</remarks>
        /// <returns>The next floor destination. If no destinations are scheduled, returns the current floor.</returns>
        private int GetNextDestination()
        {
            lock (_lock)
            {
                // (Same simple scheduling logic as before)
                if (Direction == Direction.Up || Direction == Direction.Idle)
                {
                    var upStops = _destinations.Where(f => f >= CurrentFloor).OrderBy(f => f);
                    if (upStops.Any()) return upStops.First();
                    var downStops = _destinations.Where(f => f < CurrentFloor).OrderByDescending(f => f);
                    if (downStops.Any()) return downStops.First();
                }
                else
                {
                    var downStops = _destinations.Where(f => f <= CurrentFloor).OrderByDescending(f => f);
                    if (downStops.Any()) return downStops.First();
                    var upStops = _destinations.Where(f => f > CurrentFloor).OrderBy(f => f);
                    if (upStops.Any()) return upStops.First();
                }
                return CurrentFloor;
            }
        }
    }
}
