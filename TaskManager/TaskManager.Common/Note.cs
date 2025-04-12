using System.Text.Json.Serialization;

namespace TaskManager.Common;

public class Note
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("text")] public required string Text { get; set; }
}