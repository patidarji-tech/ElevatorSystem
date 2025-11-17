using ElevatorSystem.Abstraction;
using ElevatorSystem.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorTest.Mock
{
    public class TestableElevatorCar : IElevatorCar
    {
        public int Id { get; }
        public int CurrentFloor { get; private set; }
        public Direction Direction { get; } = Direction.Idle;
        private HashSet<int> _destinations = new HashSet<int>();
        public IReadOnlyCollection<int> Destinations => _destinations;
        public TestableElevatorCar(int id, int initialFloor) { Id = id; CurrentFloor = initialFloor; }
        public void AddDestination(int floor) => _destinations.Add(floor);
        public Task OperateAsync() => Task.CompletedTask; // N/A for scheduler test
    }
}
