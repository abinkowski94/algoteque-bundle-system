using AlgotequeCourseBundler.Domain.Exceptions;

namespace AlgotequeCourseBundler.Domain.Monads;

public readonly struct Result<T> : IEquatable<Result<T>>
{
    private readonly T? _value;

    private readonly Exception? _error;

    public T Value => ToT();

    public Exception Error => ToException();

    public readonly bool HasValue => _value is not null;

    public readonly bool HasError => _error is not null;

    public Result()
    {
        _error = new ResultException("Empty result");
    }

    public Result(T value)
    {
        _value = value;
    }

    public Result(Exception error)
    {
        _error = error;
    }

    public readonly bool Equals(Result<T> other)
    {
        if (_value is null && other._value is null)
        {
            return Equals(_error, other._error);
        }

        if (_error is null && other._error is null)
        {
            return Equals(_value, other._value);
        }

        return false;
    }

    public override readonly bool Equals(object? obj)
    {
        if (obj is not Result<T> other)
        {
            return false;
        }

        return Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(_value, _error);
    }

    public static Result<T> FromT(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> FromException(Exception error)
    {
        return new Result<T>(error);
    }

    public readonly T ToT()
    {
        return _value ?? throw Error;
    }

    public readonly Exception ToException()
    {
        return _error ?? throw new ResultException("The result contains no errors.");
    }

    public static bool operator ==(Result<T> left, Result<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Result<T> left, Result<T> right)
    {
        return !(left == right);
    }

    public static implicit operator Result<T>(T value)
    {
        return FromT(value);
    }

    public static implicit operator Result<T>(Exception error)
    {
        return new Result<T>(error);
    }

    public static implicit operator T(Result<T> result)
    {
        return result.ToT();
    }

    public static implicit operator Exception(Result<T> result)
    {
        return result.ToException();
    }
}
