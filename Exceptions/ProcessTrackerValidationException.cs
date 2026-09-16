namespace ProcessTracker.API.Exceptions;

public class ProcessTrackerValidationException : ProcessTrackerException
{
    public ProcessTrackerValidationException() { }

    public ProcessTrackerValidationException(string message)
        : base(message) { }

    public ProcessTrackerValidationException(string message, System.Exception innerException)
        : base(message, innerException) { }
}
