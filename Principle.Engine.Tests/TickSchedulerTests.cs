using Moq;
using Principle.Contracts;
using Shouldly;

namespace Principle.Engine.Tests;

public class TickSchedulerTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(-30)]
    [InlineData(0)]
    [InlineData(129)]
    [InlineData(230)]
    public void AddTickSchedule_CreateWithInvalidTickRate_ThrowsException(int tickRate)
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() =>
            tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object, tickRate));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    [InlineData(60)]
    [InlineData(103)]
    [InlineData(128)]
    public void AddTickSchedule_CreateWithValidTickRate_DoesNotThrow(int tickRate)
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        // Act & Assert
        Should.NotThrow(() => tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object, tickRate));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("    ")]
    public void AddTickSchedule_CreateWithEmptyName_ThrowsException(string name)
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            tickScheduler.AddTickSchedule(name, mockTickSchedule.Object, 20));
    }

    [Theory]
    [InlineData("Schedule1")]
    [InlineData("Schedule2")]
    public void AddTickSchedule_CreateWithValidName_DoesNotThrow(string name)
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        // Act & Assert
        Should.NotThrow(() => tickScheduler.AddTickSchedule(name, mockTickSchedule.Object, 20));
    }

    [Fact]
    public void AddTickSchedule_CreateWithOverwriteDisabled_ThrowsException()
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object, 20);
            tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object, 20);
        });
    }
}
