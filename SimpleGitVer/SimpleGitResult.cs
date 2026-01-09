namespace Cake.SimpleGitVer;

public sealed record SimpleGitResult(
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
