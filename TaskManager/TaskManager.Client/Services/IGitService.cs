using System.Diagnostics.CodeAnalysis;
using TaskManager.Common;

namespace TaskManager.Client.Services;

public interface IGitService
{
    bool IsGitInstalled();

    bool TryGetOrigin(string absolutePath, [MaybeNullWhen(false)] out string origin);

    bool TryGetBaseCommitHash(string absolutePath, [MaybeNullWhen(false)] out string baseCommitHash);

    TemporaryBranch CreateTemporaryBranch(LocalRepository repository);

    void Restore(TemporaryBranch temporaryBranch, LocalRepository localRepository);
}