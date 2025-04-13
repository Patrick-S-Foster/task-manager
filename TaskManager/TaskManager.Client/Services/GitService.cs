using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
}