using ElevatorSystem.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.ElevatorImplementation
{
    public class SystemClock : IClock
    {
        /// <summary>
        /// Asynchronously delays execution for a specified number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds to delay. Must be non-negative.</param>
        /// <returns>A task that represents the asynchronous delay operation.</returns>
        public Task DelayAsync(int seconds) => Task.Delay(seconds * 1000);
    }
}
