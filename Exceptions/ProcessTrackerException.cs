using System;

namespace ProcessTracker.API.Exceptions;

public class ProcessTrackerException : Exception
{
    public ProcessTrackerException() { }

    public ProcessTrackerException(string message)
        : base(message) { }

    public ProcessTrackerException(string message, Exception innerException)
        : base(message, innerException) { }
}
