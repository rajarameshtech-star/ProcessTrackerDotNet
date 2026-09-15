namespace ProcessTracker.API.Entities;

public class Application
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Project? Project { get; set; }
    public ICollection<ServiceItem> ServiceItems { get; set; } = new List<ServiceItem>();
}
