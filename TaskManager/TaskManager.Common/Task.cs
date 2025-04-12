using System.Text.Json.Serialization;

namespace TaskManager.Common;

public class Task
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("name")] public required string Name { get; set; }

    [JsonPropertyName("description")] public required string Description { get; set; }

    [JsonPropertyName("notes")] public List<Note> Notes { get; init; } = [];

    [JsonPropertyName("branches")] public List<TemporaryBranch> Branches { get; init; } = [];

    [JsonPropertyName("state")] public required TaskState State { get; set; }

    /// <summary>
    /// The last date and time at which this task was started or resumed.
    /// </summary>
    [JsonPropertyName("lastStart")]
    public required DateTime? LastStart { get; set; }

    /// <summary>
    /// The time spent on the task, not including the time from <see cref="LastStart"/> until now.
    /// </summary>
    [JsonPropertyName("duration")]
    public required TimeSpan Duration { get; set; }
}