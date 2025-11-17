// See https://aka.ms/new-console-template for more information
using ElevatorSystem;
using ElevatorSystem.Abstraction;
using ElevatorSystem.ElevatorImplementation;

Console.WriteLine("Hello, Net ROADShoW!");

Console.WriteLine("Elevator Simulation Started (10 floors, 4 elevators). Press Ctrl+C to exit.");

// Setting up the dependencies
var clock = new SystemClock();
var logger = new ConsoleLogger();
var elevators = new List<IElevatorCar>
        {
            new ElevatorCar(1, clock, logger),
            new ElevatorCar(2, clock, logger),
            new ElevatorCar(3, clock, logger),
            new ElevatorCar(4, clock, logger)
        };
IElevatorScheduler scheduler = new SimpleElevatorScheduler(logger);

var system = new ControlRoom(elevators, scheduler, clock, logger);
await system.StartAsync();
