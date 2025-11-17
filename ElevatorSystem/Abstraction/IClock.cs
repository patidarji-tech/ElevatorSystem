using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.Abstraction
{
    /// <summary>
    /// Represents a clock that provides asynchronous delay functionality.
    /// </summary>
    /// <remarks>This interface defines a method for introducing a delay in asynchronous operations.</remarks>
    public interface IClock
    {
        Task DelayAsync(int seconds);

    }
}
