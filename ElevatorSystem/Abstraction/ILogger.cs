using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.Abstraction
{
    /// <summary>
    /// Provides a mechanism for logging messages.
    /// </summary>
    /// <remarks>This interface defines a method for logging messages, allowing different implementations to
    /// handle the logging process. Implementations may log messages to various outputs such as console, file, or
    /// external logging services.</remarks>
    public interface ILogger
    {
        void Log(string message);
    }
}
