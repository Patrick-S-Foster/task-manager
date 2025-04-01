namespace TaskManager.Common;

public class Task
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required ICollection<string> Notes { get; set; }

    public required ICollection<TemporaryBranch> Branches { get; set; }

    public required TaskState State { get; set; }

    /// <summary>
    /// The last date and time at which this task was started or resumed.
    /// </summary>
    public required DateTime? LastStart { get; set; }

    /// <summary>
    /// The time spent on the task, not including the time from <see cref="LastStart"/> until now.
    /// </summary>
    public required TimeSpan Duration { get; set; }
}