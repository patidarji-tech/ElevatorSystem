using System.Threading.Tasks;
using ElevatorSystem.ElevatorImplementation;
using ElevatorTest.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ElevatorTest
{
    /// <summary>
    /// Contains unit tests for the <see cref="ElevatorCar.OperateAsync"/> method.
    /// </summary>
    /// <remarks>This test class verifies the behavior of the <see cref="ElevatorCar"/> when handling
    /// passenger pickup and destination requests. It ensures that the elevator processes destinations correctly and
    /// logs the expected events.</remarks>
    [TestClass]
    public class OperateAsyncTests
    {
        [TestMethod]
        public async Task OperateAsync_PickupThenDestination_AddedAfterArrival()
        {
            // Arrange
            var mockClock = new MockClock();
            var mockLogger = new MockLogger();
            var elevator = new ElevatorCar(1, mockClock, mockLogger);

            // Simulate a passenger waiting at floor 3 who will request floor 7 after boarding
            int pickupFloor = 3;
            int passengerDestination = 7;

            // Subscribe to FloorArrived to simulate passenger boarding and selecting destination
            elevator.FloorArrived += (car, floor) =>
            {
                if (floor == pickupFloor)
                {
                    mockLogger.Log($"Test: Passenger boards at floor {floor} and requests {passengerDestination}");
                    car.AddDestination(passengerDestination);
                }
            };

            // Add the pickup floor only
            elevator.AddDestination(pickupFloor);

            // Act
            var operateTask = elevator.OperateAsync();

            // Since MockClock.DelayAsync returns completed tasks immediately, the elevator
            // should process its destinations synchronously. Give the loop a brief chance to run.
            await Task.Delay(10);

            // Stop the elevator loop so test can complete deterministically
            elevator.Stop();
            await operateTask;

            // Assert
            // The elevator should have arrived at the passenger destination (7)
            Assert.AreEqual(passengerDestination, elevator.CurrentFloor);
            Assert.IsFalse(elevator.Destinations.Contains(pickupFloor));
            Assert.IsFalse(elevator.Destinations.Contains(passengerDestination));

            // Verify logger captured expected events
            Assert.IsTrue(mockLogger.Logs.Any(l => l.Contains($"Stopping at floor {pickupFloor}")));
            Assert.IsTrue(mockLogger.Logs.Any(l => l.Contains("Doors close")));
            Assert.IsTrue(mockLogger.Logs.Any(l => l.Contains($"Arrived at floor {passengerDestination}")));
        }
    }
}
