namespace ProcessTracker.API.Validators;

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ValidationResult
{
    public bool IsValid => !Errors.Any();
    public List<ValidationError> Errors { get; set; } = new List<ValidationError>();

    public void AddError(string field, string message)
    {
        Errors.Add(new ValidationError { Field = field, Message = message });
    }
}
