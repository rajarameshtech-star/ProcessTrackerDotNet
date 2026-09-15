namespace ProcessTracker.API.Entities;

public class ProcessDefinition
{
    public int Id { get; set; }
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }

    public ICollection<ProcessField> ProcessFields { get; set; } = new List<ProcessField>();
    public ICollection<ProcessDefinitionProjectMapping> ProcessDefinitionProjectMappings { get; set; } = new List<ProcessDefinitionProjectMapping>();
    public ICollection<ServiceItem> ServiceItems { get; set; } = new List<ServiceItem>();
    public ICollection<ProcessRecord> ProcessRecords { get; set; } = new List<ProcessRecord>();
}
