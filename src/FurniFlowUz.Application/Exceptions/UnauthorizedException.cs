namespace FurniFlowUz.Application.Exceptions;

/// <summary>
/// Exception thrown when a user is not authorized to perform an operation
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException() : base("Unauthorized access.")
    {
    }
}
