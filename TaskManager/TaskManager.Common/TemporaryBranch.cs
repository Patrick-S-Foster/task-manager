namespace TaskManager.Common;

public class TemporaryBranch
{
    public int Id { get; set; }

    public required Repository Repository { get; set; }

    public required string Name { get; set; }

    public required string? HeadCommitHash { get; set; }

    public required string BaseCommitHash { get; set; }
}