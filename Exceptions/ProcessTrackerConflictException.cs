namespace ProcessTracker.API.Exceptions;

public class ProcessTrackerConflictException : ProcessTrackerException
{
    public ProcessTrackerConflictException() { }

    public ProcessTrackerConflictException(string message)
        : base(message) { }

    public ProcessTrackerConflictException(string message, System.Exception innerException)
        : base(message, innerException) { }
}
