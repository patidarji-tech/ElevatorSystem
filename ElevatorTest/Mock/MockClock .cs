using ElevatorSystem.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorTest.Mock
{
    internal class MockClock : IClock
    {
        // In a real test mock, you'd manage tasks better. 
        // Here we just return a completed task immediately so logic executes synchronously in the test
        public Task DelayAsync(int seconds) => Task.CompletedTask;
    }
   
}
