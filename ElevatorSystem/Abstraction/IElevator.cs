using ElevatorSystem.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.Abstraction
{
 /// <summary>
 /// Represents an elevator car within a building's elevator system.
 /// </summary>
 /// <remarks>This interface defines the basic operations and properties of an elevator car,  including its
 /// current state and the ability to manage its destinations.</remarks>
    public interface IElevatorCar
    {
        int Id { get; }
        int CurrentFloor { get; }
        Direction Direction { get; }
        IReadOnlyCollection<int> Destinations { get; }
        void AddDestination(int floor);
        Task OperateAsync();
    }
}
