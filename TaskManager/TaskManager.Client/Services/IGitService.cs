using System.Diagnostics.CodeAnalysis;

namespace TaskManager.Client.Services;

public interface IGitService
{
    bool IsGitInstalled();

    bool TryGetOrigin(string absolutePath, [MaybeNullWhen(false)] out string origin);
}