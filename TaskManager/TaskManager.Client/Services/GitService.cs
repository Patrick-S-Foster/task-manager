using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TaskManager.Common;

namespace TaskManager.Client.Services;

public class GitService : IGitService
{
    public bool IsGitInstalled()
    {
        using var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        process.StandardInput.WriteLine("git version");
        process.StandardInput.Close();
        process.WaitForExit();

        return process.StandardError.ReadToEnd().Trim().Length is 0;
    }

    public bool TryGetOrigin(string absolutePath, [MaybeNullWhen(false)] out string origin)
    {
        const string getRemoteUrlCommand = "git remote get-url --push origin";

        using var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        process.StandardInput.WriteLine($"cd {absolutePath}");
        process.StandardInput.WriteLine(getRemoteUrlCommand);
        process.StandardInput.Close();
        process.WaitForExit();

        if (process.StandardError.ReadToEnd().Trim().Length is not 0)
        {
            origin = null;
            return false;
        }

        var output = process.StandardOutput.ReadToEnd();
        var foundCommand = false;

        foreach (var line in output.Split('\n'))
        {
            if (foundCommand)
            {
                origin = line.Trim();
                return true;
            }

            if (line.Contains(getRemoteUrlCommand))
            {
                foundCommand = true;
            }
        }

        origin = null;
        return false;
    }

    public bool TryGetBaseCommitHash(string absolutePath, [MaybeNullWhen(false)] out string baseCommitHash)
    {
        const string getBaseCommitCommand = "git rev-parse HEAD";

        using var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        process.StandardInput.WriteLine($"cd {absolutePath}");
        process.StandardInput.WriteLine(getBaseCommitCommand);
        process.StandardInput.Close();
        process.WaitForExit();

        if (process.StandardError.ReadToEnd().Trim().Length is not 0)
        {
            baseCommitHash = null;
            return false;
        }

        var output = process.StandardOutput.ReadToEnd();
        var foundCommand = false;

        foreach (var line in output.Split('\n'))
        {
            if (foundCommand)
            {
                baseCommitHash = line.Trim();
                return true;
            }

            if (line.Contains(getBaseCommitCommand))
            {
                foundCommand = true;
            }
        }

        baseCommitHash = null;
        return false;
    }

    public TemporaryBranch CreateTemporaryBranch(Repository repository)
    {
        if (repository is not { LocalPath: { } localPath } ||
            !TryGetBaseCommitHash(localPath, out var baseCommitHash))
        {
            throw new InvalidOperationException();
        }

        var guid = Guid.NewGuid().ToString();
        var createNewBranchCommand = $"git switch -c task-manager/{guid}";
        const string addCommand = "git add .";
        const string commitCommand = "git commit -am \"Temporary commit for task manager.\"";
        var pushCommand = $"git push --set-upstream origin task-manager/{guid}";

        using var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        process.StandardInput.WriteLine($"cd {localPath}");
        process.StandardInput.WriteLine(createNewBranchCommand);
        process.StandardInput.WriteLine(addCommand);
        process.StandardInput.WriteLine(commitCommand);
        process.StandardInput.WriteLine(pushCommand);
        process.StandardInput.Close();
        process.WaitForExit();

        if (!TryGetBaseCommitHash(localPath, out var headCommitHash))
        {
            throw new InvalidOperationException();
        }

        return new TemporaryBranch
        {
            Repository = repository,
            Name = $"task-manager/{guid}",
            HeadCommitHash = headCommitHash,
            BaseCommitHash = baseCommitHash
        };
    }

    public void Restore(TemporaryBranch temporaryBranch)
    {
        var switchCommand = $"git switch {temporaryBranch.Repository.Name}";
        var resetCommand = $"git reset {temporaryBranch.BaseCommitHash}";

        using var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        process.StandardInput.WriteLine($"cd {temporaryBranch.Repository.LocalPath}");
        process.StandardInput.WriteLine(switchCommand);
        process.StandardInput.WriteLine(resetCommand);
        process.StandardInput.Close();
        process.WaitForExit();
    }
}