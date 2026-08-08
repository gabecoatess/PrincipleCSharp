using Moq;
using Principle.Contracts;
using Shouldly;

namespace Principle.Engine.Tests;

public class TickSchedulerTests
{
    [Theory]
    [InlineData(-30.0)]
    [InlineData(-1.0)]
    [InlineData(0.0)]
    [InlineData(129.0)]
    [InlineData(230.0)]
    public void AddTickSchedule_CreateWithInvalidTickRate_ThrowsException(double tickRate)
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() =>
            tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object, tickRate));
    }

    [Theory]
    [InlineData(0.2)]
    [InlineData(1.0)]
    [InlineData(20.0)]
    [InlineData(60.0)]
    [InlineData(103.5)]
    [InlineData(128.0)]
    public void AddTickSchedule_CreateWithValidTickRate_DoesNotThrow(double tickRate)
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        // Act & Assert
        Should.NotThrow(() => tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object, tickRate));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    public void AddTickSchedule_CreateWithEmptyName_ThrowsException(string name)
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            tickScheduler.AddTickSchedule(name, mockTickSchedule.Object));
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
        Should.NotThrow(() => tickScheduler.AddTickSchedule(name, mockTickSchedule.Object));
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
            tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object);
            tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object);
        });
    }

    [Fact]
    public void AddTickSchedule_CreateWithOverwriteEnabled_DoesNotThrow()
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        // Act & Assert
        Should.NotThrow(() =>
        {
            tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object);
            tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object, overwrite: true);
        });
    }

    [Fact]
    public void AddTickSchedule_CreateWithOverwriteEnabled_TickRateOverwritten()
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        // Act
        tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object);
        tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object, 33.3, overwrite: true);

        // Assert
        tickScheduler.GetTickScheduleTickRate("Schedule").ShouldBe(33.3);
    }

    [Fact]
    public void AddTickSchedule_CreateWithNoTickRateSpecified_DefaultTickRateApplied()
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        // Act
        tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object);

        // Assert
        tickScheduler.GetTickScheduleTickRate("Schedule").ShouldBe(20.0);
    }

    [Fact]
    public void TryGetTickSchedule_LookupExistingScheduleByName_RecoverExistingSchedule()
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object);

        // Act
        var result = tickScheduler.TryGetTickSchedule("Schedule", out var retrievedSchedule);

        // Assert
        result.ShouldBeTrue();
        retrievedSchedule.ShouldNotBeNull();
    }

    [Fact]
    public void RemoveTickSchedule_TryToRemoveRegisteredSchedule_RegisteredScheduleIsRemoved()
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var mockTickSchedule = new Mock<ITickSchedule>();

        tickScheduler.AddTickSchedule("Schedule", mockTickSchedule.Object);

        // Act
        var result = tickScheduler.RemoveTickSchedule("Schedule");
        var existingSchedule = tickScheduler.TryGetTickSchedule("Schedule", out var retrievedSchedule);

        // Assert
        result.ShouldBeTrue();
        existingSchedule.ShouldBeFalse();
        retrievedSchedule.ShouldBeNull();
    }
}
