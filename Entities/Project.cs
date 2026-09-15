namespace ProcessTracker.API.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<ProcessDefinitionProjectMapping> ProcessDefinitionProjectMappings { get; set; } = new List<ProcessDefinitionProjectMapping>();
}
