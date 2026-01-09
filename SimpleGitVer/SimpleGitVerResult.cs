namespace Cake.SimpleGitVer;

/// <summary>
/// Represents the result of a Git versioning operation, containing various version information
/// obtained from the Git repository. This includes semantic version components such as major,
/// minor, and patch versions, as well as additional Git-specific details like the commit SHA,
/// dirty state, and descriptive tags.
/// </summary>
/// <remarks>
/// The versioning result is typically derived from Git tags and the `git describe` command,
/// supplemented by metadata such as number of commits ahead of the base version and custom tag prefixes.
/// </remarks>
public sealed record SimpleGitVerResult(
    int Major,
    int Minor,
    int Patch,
    int BaseBuild,
    int FinalBuild,
    int CommitsAhead,
    string Sha,
    bool IsDirty,
    string RawDescribe,
    string TagPrefix)
{
    public Version BaseVersion => new(Major, Minor, Patch, BaseBuild);
    public Version Version => new(Major, Minor, Patch, FinalBuild);
}
