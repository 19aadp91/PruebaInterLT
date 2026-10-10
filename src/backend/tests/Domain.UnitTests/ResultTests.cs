using Domain.Common;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests;

public class ResultTests
{
    [Fact]
    public void Success_WithData_ShouldSetPropertiesCorrectly()
    {
        // Act
        var result = Result<string>.Success("test payload", "Custom message");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Custom message");
        result.Data.Should().Be("test payload");
        result.ErrorType.Should().Be(ErrorType.None);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithErrorTypeAndErrors_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2" };

        // Act
        var result = Result<int>.Failure("Falló la operación", ErrorType.Validation, errors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Falló la operación");
        result.Data.Should().Be(0);
        result.ErrorType.Should().Be(ErrorType.Validation);
        result.Errors.Should().HaveCount(2).And.Contain("Error 1");
    }

    [Fact]
    public void NonGenericResult_SuccessAndFailure_ShouldWorkProperly()
    {
        // Act
        var success = Result.Success("Todo bien");
        var failure = Result.Failure("No encontrado", ErrorType.NotFound);

        // Assert
        success.IsSuccess.Should().BeTrue();
        success.ErrorType.Should().Be(ErrorType.None);
        success.Data.Should().BeNull();

        failure.IsSuccess.Should().BeFalse();
        failure.ErrorType.Should().Be(ErrorType.NotFound);
        failure.Message.Should().Be("No encontrado");
    }
}
