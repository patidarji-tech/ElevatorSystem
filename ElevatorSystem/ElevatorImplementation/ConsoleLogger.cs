using ElevatorSystem.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.ElevatorImplementation
{
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Logs the specified message to the console.
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
