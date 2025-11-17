using ElevatorSystem.Abstraction;
using ElevatorSystem.ElevatorImplementation;
using ElevatorTest.Mock;

namespace ElevatorTest
{
    [TestClass]
    public sealed class Test1
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // This method is called once for the test class, before any tests of the class are run.
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // This method is called once for the test class, after all tests of the class are run.
        }

        [TestInitialize]
        public void TestInit()
        {
            // This method is called before each test method.
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // This method is called after each test method.
        }

        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(true);
        }


        [TestMethod]
        public void Scheduler_AssignsCallToClosestElevator()
        {
            // Arrange
            var mockClock = new MockClock();
            var mockLogger = new MockLogger();
            var scheduler = new SimpleElevatorScheduler(mockLogger);
            var elevator1 = new ElevatorCar(1, mockClock, mockLogger);
            var elevator2 = new ElevatorCar(2, mockClock, mockLogger); // Defaults to floor 1

            // Manually set up state for the test scenario
            // Let's pretend Elevator 1 is at floor 5
            // (In a real mock setup, you'd likely make CurrentFloor settable via a mock interface)
            // For this example, we assume we can control initial state:
            // (Note: The current ElevatorCar implementation doesn't allow setting CurrentFloor externally after creation)

            // A better approach for testing the scheduler alone:
            var mockElevators = new List<IElevatorCar>
    {
        new TestableElevatorCar(1, 1), // At floor 1
        new TestableElevatorCar(2, 5)  // At floor 5
    };

            // Act
            // Passenger at floor 8 wants to go to floor 10
            scheduler.AssignCall(mockElevators, 8);

            // Assert
            // Elevator 2 is closer to floor 8 (3 floors away vs 7 for Elevator 1)
            Assert.IsTrue(mockElevators.First(e => e.Id == 2).Destinations.Contains(8));
            Assert.IsTrue(mockElevators.First(e => e.Id == 2).Destinations.Contains(10));
            Assert.AreEqual(mockElevators.First(e => e.Id == 1).Destinations.Count, 0);
        }

    }
}
