using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TaskManager.Common;

public class TemporaryBranch
{
    [JsonPropertyName("id"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [JsonPropertyName("repository")] public required Repository Repository { get; set; }

    [JsonPropertyName("name")] public required string Name { get; set; }

    [JsonPropertyName("headCommitHash")] public required string? HeadCommitHash { get; set; }

    [JsonPropertyName("baseCommitHash")] public required string BaseCommitHash { get; set; }
}