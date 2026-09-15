namespace ProcessTracker.API.Entities;

public class ProcessDefinitionProjectMapping
{
    public int ProcessDefinitionId { get; set; }
    public int ProjectId { get; set; }

    public ProcessDefinition? ProcessDefinition { get; set; }
    public Project? Project { get; set; }
}
