using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.Abstraction
{
    /// <summary>
    /// Defines a contract for scheduling elevator calls to available elevator cars.
    /// </summary>
    /// <remarks>Implementations of this interface are responsible for determining which elevator car should
    /// respond to a call from a specified floor.</remarks>
    public interface IElevatorScheduler
    {
        void AssignCall(IEnumerable<IElevatorCar> elevators, int callFloor);
    }
}
