using System.Text.RegularExpressions;
using Cake.Common;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Cake.SimpleGitVer;

/// <summary>
/// Provides Cake aliases for retrieving semantic versioning information
/// from a Git repository using the `git describe` command with a specified
/// tag prefix and optional auto-increment build configuration.
/// </summary>
[CakeAliasCategory("SimpleGitVer")]
public static class CakeSimpleGitVer
{
    /// <summary>
    /// Retrieves a Git versioning result using the `git describe` command based on the specified settings.
    /// Parses the Git output to extract versioning information such as major, minor, patch, build numbers,
    /// commit SHA, and dirty state, applying optional build incrementing.
    /// </summary>
    /// <param name="context">The Cake context used to execute the Git process and log messages.</param>
    /// <param name="settings">Settings that configure the Git command execution, including tag prefix,
    /// auto-increment behavior, and the Git executable path.</param>
    /// <returns>A <see cref="SimpleGitVerResult"/> containing parsed version information from the Git repository.</returns>
    /// <exception cref="CakeException">
    /// Thrown when the Git command execution fails, produces invalid output, or the Git tag format does not match the expected pattern.
    /// </exception>
    [CakeMethodAlias]
    public static SimpleGitVerResult GetSimpleGitVer(this ICakeContext context, SimpleGitVerSettings settings)
    {
        context.Log.Write(Verbosity.Verbose, LogLevel.Information, $"Tag prefix: {settings.TagPrefix}");
        var procSettings = new ProcessSettings();
        procSettings.RedirectStandardOutput = true;
        string returnedStringFromGit = null;
        procSettings.RedirectedStandardOutputHandler = s =>
        {
            if (String.IsNullOrEmpty(s))
            {
                return s;
            }
            if (returnedStringFromGit != null)
            {
                context.Log.Write(Verbosity.Normal, LogLevel.Error, "Git returned invalid data: {0}   {1}", returnedStringFromGit, s);
                throw new CakeException("git describe failed");
            }
            returnedStringFromGit = s;
            return s;
        };
        var args = new ProcessArgumentBuilder();
        args.Append("describe");
        args.Append("--tags");
        args.AppendSwitchQuoted("--match", settings.TagPrefix + "*");
        procSettings.Arguments = args;
        var exitCode = context.StartProcess(new FilePath(settings.GitExe), procSettings);
        if (exitCode != 0)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Error, "Failed to execute Git, returned exit code: {0}",
                exitCode);
            throw new CakeException("git describe failed");
        }

        if (String.IsNullOrEmpty(returnedStringFromGit))
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Error, "Git returned invalid data");
            throw new CakeException("git describe failed");
        }
        
        context.Log.Write(Verbosity.Verbose, LogLevel.Information, $"Tag received from git: {returnedStringFromGit}");

        var rx = new Regex(
            @$"^{settings.TagPrefix}" +
            @"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)\.(?<build>\d+)" +
            @"(?:-(?<ahead>\d+)-g(?<sha>[0-9a-f]+))?" +
            @"(?<dirty>-dirty)?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var m = rx.Match(returnedStringFromGit);
        if (!m.Success)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Error, "Invalid git tag format");
            throw new CakeException("Invalid git tag format");
        }
        
        var major = int.Parse(m.Groups["major"].Value);
        var minor = int.Parse(m.Groups["minor"].Value);
        var patch = int.Parse(m.Groups["patch"].Value);
        var build = int.Parse(m.Groups["build"].Value);
        int commitsAhead = m.Groups["ahead"].Success ? int.Parse(m.Groups["ahead"].Value) : 0;
        string sha = m.Groups["sha"].Success ? m.Groups["sha"].Value : "";
        bool dirty = m.Groups["dirty"].Success;
        var finalBuild = settings.AutoIncrementBuildNumber ? (build + commitsAhead) : build;
        return new SimpleGitVerResult(major, minor, patch, build, finalBuild, commitsAhead, sha, dirty, returnedStringFromGit, settings.TagPrefix);
    }
}