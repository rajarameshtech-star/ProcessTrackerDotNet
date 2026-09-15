namespace ProcessTracker.API.Entities;

public class ProcessField
{
    public int Id { get; set; }
    public int ProcessDefinitionId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public FieldType FieldType { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public string? Placeholder { get; set; }
    public string? DefaultValue { get; set; }
    public string? OptionsJson { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public bool IsActive { get; set; }

    public ProcessDefinition? ProcessDefinition { get; set; }
}
