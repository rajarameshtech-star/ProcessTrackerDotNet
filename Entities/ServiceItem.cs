namespace ProcessTracker.API.Entities;

public class ServiceItem
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public int ProcessDefinitionId { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Application? Application { get; set; }
    public ProcessDefinition? ProcessDefinition { get; set; }
    public ProcessRecord? ProcessRecord { get; set; }
}
