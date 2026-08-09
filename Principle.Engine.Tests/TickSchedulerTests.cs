using Moq;
using Principle.ECS;
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

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() =>
            tickScheduler.AddTickSchedule("Schedule", tickRate));
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

        // Act & Assert
        Should.NotThrow(() => tickScheduler.AddTickSchedule("Schedule", tickRate));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    public void AddTickSchedule_CreateWithEmptyName_ThrowsException(string name)
    {
        // Arrange
        var tickScheduler = new TickScheduler();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            tickScheduler.AddTickSchedule(name));
    }

    [Theory]
    [InlineData("Schedule1")]
    [InlineData("Schedule2")]
    public void AddTickSchedule_CreateWithValidName_DoesNotThrow(string name)
    {
        // Arrange
        var tickScheduler = new TickScheduler();

        // Act & Assert
        Should.NotThrow(() => tickScheduler.AddTickSchedule(name));
    }

    [Fact]
    public void AddTickSchedule_CreateWithOverwriteDisabled_ThrowsException()
    {
        // Arrange
        var tickScheduler = new TickScheduler();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            tickScheduler.AddTickSchedule("Schedule");
            tickScheduler.AddTickSchedule("Schedule");
        });
    }

    [Fact]
    public void AddTickSchedule_CreateWithOverwriteEnabled_DoesNotThrow()
    {
        // Arrange
        var tickScheduler = new TickScheduler();

        // Act & Assert
        Should.NotThrow(() =>
        {
            tickScheduler.AddTickSchedule("Schedule");
            tickScheduler.AddTickSchedule("Schedule", overwrite: true);
        });
    }

    [Fact]
    public void AddTickSchedule_CreateWithOverwriteEnabled_TickRateOverwritten()
    {
        // Arrange
        var tickScheduler = new TickScheduler();

        // Act
        tickScheduler.AddTickSchedule("Schedule");
        tickScheduler.AddTickSchedule("Schedule", 33.3, overwrite: true);

        // Assert
        tickScheduler.GetTickScheduleTickRate("Schedule").ShouldBe(33.3);
    }

    [Fact]
    public void AddTickSchedule_CreateWithNoTickRateSpecified_DefaultTickRateApplied()
    {
        // Arrange
        var tickScheduler = new TickScheduler();

        // Act
        tickScheduler.AddTickSchedule("Schedule");

        // Assert
        tickScheduler.GetTickScheduleTickRate("Schedule").ShouldBe(20.0);
    }

    [Fact]
    public void AddTickSchedule_OverwriteExistingSchedule_ReplacesScheduledBehavior()
    {
        // Arrange
        var tickScheduler = new TickScheduler();
        var systemA = new Mock<IPrincipleSystem>();
        var systemB = new Mock<IPrincipleSystem>();

        var tickSchedule = tickScheduler.AddTickSchedule("Schedule");
        tickSchedule.AddSystem("SystemA", systemA.Object);

        // Act
        var newSchedule = tickScheduler.AddTickSchedule("Schedule", overwrite: true);
        newSchedule.AddSystem("SystemB", systemB.Object);

        // Assert
        newSchedule.ShouldNotBeSameAs(tickSchedule);
        newSchedule.GetSystem("SystemA").ShouldBeNull();
        newSchedule.GetSystem("SystemB").ShouldBeSameAs(systemB.Object);
    }

    [Fact]
    public void TryGetTickSchedule_LookupExistingScheduleByName_RecoverExistingSchedule()
    {
        // Arrange
        var tickScheduler = new TickScheduler();

        tickScheduler.AddTickSchedule("Schedule");

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

        tickScheduler.AddTickSchedule("Schedule");

        // Act
        var result = tickScheduler.RemoveTickSchedule("Schedule");
        var existingSchedule = tickScheduler.TryGetTickSchedule("Schedule", out var retrievedSchedule);

        // Assert
        result.ShouldBeTrue();
        existingSchedule.ShouldBeFalse();
        retrievedSchedule.ShouldBeNull();
    }

    [Fact]
    public void RemoveTickSchedule_TryToRemoveNotRegisteredSchedule_ReturnsFalse()
    {
        // Arrange
        var tickScheduler = new TickScheduler();

        // Act
        var result = tickScheduler.RemoveTickSchedule("Schedule");

        // Assert
        result.ShouldBeFalse();
    }
}
