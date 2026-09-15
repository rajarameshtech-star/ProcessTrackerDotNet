namespace ProcessTracker.API.Entities;

public class ProcessRecord
{
    public int Id { get; set; }
    public int ServiceItemId { get; set; }
    public int ProcessDefinitionId { get; set; }
    public string DataJson { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    public ServiceItem? ServiceItem { get; set; }
    public ProcessDefinition? ProcessDefinition { get; set; }
}
