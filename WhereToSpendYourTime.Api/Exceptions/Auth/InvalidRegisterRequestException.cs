namespace WhereToSpendYourTime.Api.Exceptions.Auth;

/// <summary>
/// Thrown when a register request contains invalid or incomplete data
/// </summary>
public sealed class InvalidRegisterRequestException : Exception
{
    public InvalidRegisterRequestException(string message) : base(message) { }
}