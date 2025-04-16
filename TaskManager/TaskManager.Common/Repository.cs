using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TaskManager.Common;

public class Repository
{
    [JsonPropertyName("id"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [JsonPropertyName("url")] public required string? Url { get; set; }

    [JsonPropertyName("name")] public required string Name { get; set; }
    
    [JsonPropertyName("localPath")] public required string? LocalPath { get; set; }
}