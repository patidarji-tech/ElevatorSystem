using ElevatorSystem.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorTest.Mock
{
    internal class MockLogger : ILogger
    {
        public List<string> Logs { get; } = new List<string>();
        public void Log(string message) => Logs.Add(message);
    }
}
