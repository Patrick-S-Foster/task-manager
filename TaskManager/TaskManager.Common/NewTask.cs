using System.Text.Json.Serialization;

namespace TaskManager.Common;

public class NewTask
{
    [JsonPropertyName("name")] public required string Name { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }
}