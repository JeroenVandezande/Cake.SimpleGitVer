# Cake.SimpleGitVersion

[<img src="./Cake.SimpleGitVersion/cakebadge.svg" width="100">](https://cakebuild.net/)

[![Version](https://img.shields.io/nuget/vpre/Cake.SimpleGitVersion.svg)](https://www.nuget.org/packages/Cake.SimpleGitVersion)
[![NuGet download count](https://img.shields.io/nuget/dt/Cake.SimpleGitVersion.svg)](https://www.nuget.org/packages/Cake.SimpleGitVersion)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A Cake addin to derive build versions from `git describe` and a simple, numeric tag convention.

This addin is intentionally lightweight: it shells out to the Git CLI (cross-platform) and parses the single-line output.

## Tag convention

Create tags like:

```
AIX1000-v3.1.1.1097
```

Where the tag is:

```
<TagPrefix><Major>.<Minor>.<Patch>.<Build>
```

Then `git describe` will yield (examples):

- On the tag: `AIX1000-v3.1.1.1097`
- 12 commits ahead: `AIX1000-v3.1.1.1097-12-gbb7f5a99c1`
- Dirty working tree: `AIX1000-v3.1.1.1097-12-gbb7f5a99c1-dirty`

The addin returns a rich result including `BaseBuild`, `CommitsAhead`, `Sha`, and `IsDirty`. Optionally, it can auto-increment the build component:

- `FinalBuild = BaseBuild + CommitsAhead` (when `AutoIncrementBuildNumber = true`)

## Usage

### Include an Add-In directive

```csharp
#addin "nuget:?package=Cake.SimpleGitVersion&loaddependencies=true"
```

### Example

```csharp
Task("Print-Version")
    .Does(context =>
{
    var settings = new SimpleGitVerSettings
    {
        // Example: "AIX1000-v"
        TagPrefix = "AIX1000-v",

        // Optional: path to the git executable
        GitExe = "git",

        // If true: FinalBuild = BaseBuild + CommitsAhead
        AutoIncrementBuildNumber = true
    };

    var r = context.GetSimpleGitVer(settings);

    Information("RawDescribe: {0}", r.RawDescribe);
    Information("BaseVersion: {0}", r.BaseVersion);
    Information("Version: {0}", r.Version);
    Information("CommitsAhead: {0}", r.CommitsAhead);
    Information("Sha: {0}", r.Sha);
    Information("IsDirty: {0}", r.IsDirty);
});
```

## API

### Alias

```csharp
SimpleGitResult GetSimpleGitVer(this ICakeContext context, SimpleGitVerSettings settings)
```

### Result

```csharp
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
```

### Settings

```csharp
public sealed class SimpleGitVerSettings
{
    public string TagPrefix { get; set; } = "";
    public string GitExe { get; set; } = "git";
    public bool AutoIncrementBuildNumber { get; set; } = true;
}
```

## Notes

- The addin executes: `git describe --tags --long --dirty --match "<TagPrefix>*"`
- The output is expected to be a single line. If Git produces no output or an unexpected format, the addin throws a `CakeException`.
- If your build agent does not fetch tags, `git describe` will fail. Ensure your CI checkout fetches tags.

## License

MIT
