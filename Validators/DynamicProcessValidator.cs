using System.Text.Json;
using ProcessTracker.API.Entities;

namespace ProcessTracker.API.Validators;

public class DynamicProcessValidator : IDynamicProcessValidator
{
    public ValidationResult Validate(ProcessDefinition processDefinition, string submittedJson)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(submittedJson))
        {
            result.AddError("DataJson", "DataJson is required.");
            return result;
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(submittedJson);
        }
        catch (JsonException)
        {
            result.AddError("DataJson", "DataJson must contain a valid JSON object.");
            return result;
        }

        using (document)
        {
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                result.AddError("DataJson", "DataJson must contain a valid JSON object as its root.");
                return result;
            }

            var activeFields = processDefinition.ProcessFields.Where(f => f.IsActive).ToList();
            var activeFieldDict = activeFields.ToDictionary(f => f.FieldName);

            // Check for unknown or inactive fields submitted
            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (!activeFieldDict.TryGetValue(property.Name, out var fieldDef))
                {
                    result.AddError(property.Name, $"Unknown field '{property.Name}'.");
                }
            }

            // Validate all active fields
            foreach (var field in activeFields)
            {
                bool jsonHasProperty = document.RootElement.TryGetProperty(field.FieldName, out var jsonElement) && jsonElement.ValueKind != JsonValueKind.Null;

                if (field.IsRequired && !jsonHasProperty)
                {
                    result.AddError(field.FieldName, $"Field '{field.FieldName}' is required.");
                    continue;
                }

                if (jsonHasProperty)
                {
                    ValidateFieldType(field, jsonElement, result);
                }
            }
        }

        return result;
    }

    private void ValidateFieldType(ProcessField fieldDef, JsonElement element, ValidationResult result)
    {
        switch (fieldDef.FieldType)
        {
            case FieldType.Text:
                if (element.ValueKind != JsonValueKind.String)
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' must be a string.");
                    break;
                }
                var textValue = element.GetString() ?? string.Empty;
                if (fieldDef.IsRequired && string.IsNullOrWhiteSpace(textValue))
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' is required and cannot be empty.");
                }
                if (fieldDef.MinLength.HasValue && textValue.Length < fieldDef.MinLength.Value)
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' must be at least {fieldDef.MinLength} characters.");
                }
                if (fieldDef.MaxLength.HasValue && textValue.Length > fieldDef.MaxLength.Value)
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' must be at most {fieldDef.MaxLength} characters.");
                }
                break;

            case FieldType.Number:
                if (element.ValueKind != JsonValueKind.Number || !element.TryGetInt64(out _))
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' must be a number.");
                }
                break;

            case FieldType.Decimal:
                if (element.ValueKind != JsonValueKind.Number || !element.TryGetDecimal(out _))
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' must be a decimal number.");
                }
                break;

            case FieldType.Boolean:
                if (element.ValueKind != JsonValueKind.True && element.ValueKind != JsonValueKind.False)
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' must be a boolean.");
                }
                break;

            case FieldType.Date:
                if (element.ValueKind != JsonValueKind.String || !DateOnly.TryParse(element.GetString(), out _))
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' must be a valid date.");
                }
                break;

            case FieldType.DateTime:
                if (element.ValueKind != JsonValueKind.String || !DateTime.TryParse(element.GetString(), out _))
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' must be a valid date and time.");
                }
                break;

            case FieldType.Select:
                if (element.ValueKind != JsonValueKind.String)
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' must be a string.");
                    break;
                }
                var selectValue = element.GetString();
                if (string.IsNullOrWhiteSpace(fieldDef.OptionsJson))
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' has malformed metadata (missing OptionsJson).");
                    break;
                }

                try
                {
                    var options = JsonSerializer.Deserialize<List<string>>(fieldDef.OptionsJson);
                    if (options == null || (selectValue != null && !options.Contains(selectValue)))
                    {
                        result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' has an invalid option. Allowed values: {fieldDef.OptionsJson}.");
                    }
                }
                catch (JsonException)
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' has an invalid OptionsJson format.");
                }
                break;

            case FieldType.Url:
                if (element.ValueKind != JsonValueKind.String || !Uri.TryCreate(element.GetString(), UriKind.Absolute, out _))
                {
                    result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' must be a valid absolute URL.");
                }
                break;

            default:
                result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' has an unknown field type.");
                break;
        }

        // Additional Metadata checks:
        if (fieldDef.MinLength.HasValue && fieldDef.MaxLength.HasValue && fieldDef.MinLength > fieldDef.MaxLength)
        {
            result.AddError(fieldDef.FieldName, $"Field '{fieldDef.FieldName}' has invalid metadata (MinLength > MaxLength).");
        }
    }
}
