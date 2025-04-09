namespace TaskManager.Client.Services;

public class LocalRepository(string name)
{
    public int? Id { get; set; }

    public string? Url { get; set; }

    public string? LocalPath { get; set; }

    public string Name { get; set; } = name;
}