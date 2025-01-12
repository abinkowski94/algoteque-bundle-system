using AlgotequeCourseBundler.Domain.Exceptions;
using AlgotequeCourseBundler.Domain.Monads;
using FluentAssertions.Execution;

namespace AlgotequeCourseBundler.Domain.UnitTests.Monads;

public class ResultTests
{
    [Fact]
    public void FromT_ShouldCreateResultWithValue()
    {
        // Arrange
        var value = "Test Value";

        // Act
        var result = Result<string>.FromT(value);

        // Assert
        using (new AssertionScope())
        {
            result.HasValue.Should().BeTrue();
            result.HasError.Should().BeFalse();
            result.Value.Should().Be(value);
        }
    }

    [Fact]
    public void FromException_ShouldCreateResultWithError()
    {
        // Arrange
        var exception = new InvalidOperationException("Test Exception");

        // Act
        var result = Result<string>.FromException(exception);

        // Assert
        using (new AssertionScope())
        {
            result.HasValue.Should().BeFalse();
            result.HasError.Should().BeTrue();
            result.Error.Should().Be(exception);
        }
    }

    [Fact]
    public void ToT_ShouldReturnValue_WhenResultHasValue()
    {
        // Arrange
        var value = "Test Value";
        var result = Result<string>.FromT(value);

        // Act
        var retrievedValue = result.ToT();

        // Assert
        retrievedValue.Should().Be(value);
    }

    [Fact]
    public void ToT_ShouldThrowException_WhenResultHasError()
    {
        // Arrange
        var exception = new InvalidOperationException("Test Exception");
        var result = Result<string>.FromException(exception);

        // Act
        Action act = () => result.ToT();

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("Test Exception");
    }

    [Fact]
    public void ToException_ShouldReturnError_WhenResultHasError()
    {
        // Arrange
        var exception = new InvalidOperationException("Test Exception");
        var result = Result<string>.FromException(exception);

        // Act
        var retrievedError = result.ToException();

        // Assert
        retrievedError.Should().Be(exception);
    }

    [Fact]
    public void ToException_ShouldThrowResultException_WhenResultHasValue()
    {
        // Arrange
        var value = "Test Value";
        var result = Result<string>.FromT(value);

        // Act
        Action act = () => result.ToException();

        // Assert
        act.Should().Throw<ResultException>().WithMessage("The result contains no errors.");
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_ForEqualResults()
    {
        // Arrange
        var result1 = Result<string>.FromT("Test Value");
        var result2 = Result<string>.FromT("Test Value");

        // Act & Assert
        (result1 == result2).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnFalse_ForDifferentResults()
    {
        // Arrange
        var result1 = Result<string>.FromT("Test Value 1");
        var result2 = Result<string>.FromT("Test Value 2");

        // Act & Assert
        (result1 == result2).Should().BeFalse();
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateResultWithValue()
    {
        // Arrange
        string value = "Test Value";

        // Act
        Result<string> result = value;

        // Assert
        using (new AssertionScope())
        {
            result.HasValue.Should().BeTrue();
            result.HasError.Should().BeFalse();
            result.Value.Should().Be(value);
        }
    }

    [Fact]
    public void ImplicitConversion_FromException_ShouldCreateResultWithError()
    {
        // Arrange
        var exception = new InvalidOperationException("Test Exception");

        // Act
        Result<string> result = exception;

        // Assert
        using (new AssertionScope())
        {
            result.HasValue.Should().BeFalse();
            result.HasError.Should().BeTrue();
            result.Error.Should().Be(exception);
        }
    }

    [Fact]
    public void GetHashCode_ShouldBeSame_ForEqualResults()
    {
        // Arrange
        const string value = "Test Value";
        var result1 = Result<string>.FromT(value);
        var result2 = Result<string>.FromT(value);

        // Act & Assert
        result1.GetHashCode().Should().Be(result2.GetHashCode());
    }
}
