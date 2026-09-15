using ProcessTracker.API.Entities;

namespace ProcessTracker.API.Validators;

public interface IDynamicProcessValidator
{
    ValidationResult Validate(ProcessDefinition processDefinition, string submittedJson);
}
