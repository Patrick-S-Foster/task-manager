using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Common;

public class TemporaryBranch
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public required Repository Repository { get; set; }

    public required string Name { get; set; }

    public required string? HeadCommitHash { get; set; }

    public required string BaseCommitHash { get; set; }
}