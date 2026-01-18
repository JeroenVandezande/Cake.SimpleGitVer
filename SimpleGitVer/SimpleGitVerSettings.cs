using Cake.Core.Diagnostics;
using Cake.Core.Tooling;

namespace Cake.SimpleGitVer;

/// <summary>
/// Represents the settings used to configure the SimpleGitVer tool in a Cake build process.
/// </summary>
public class SimpleGitVerSettings: ToolSettings
{
    /// <summary>
    /// Specifies the prefix used to identify relevant Git tags in the repository.
    /// This prefix is applied as a filter to match tags that should be considered
    /// by SimpleGitVer for version determination.
    /// </summary>
    public required string TagPrefix { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the build number should be automatically incremented based
    /// on the number of commits ahead of the last tagged version. Setting this property
    /// to true enables automatic incrementation of the build number, while setting it
    /// to false retains the original build number from the tag.
    /// </summary>
    public bool AutoIncrementBuildNumber { get; set; } = false;

    /// <summary>
    /// Determines whether the Patch component of the semantic version number
    /// should be automatically incremented to generate a unique version for each build.
    /// When enabled, SimpleGitVer will ensure the Patch number is updated
    /// incrementally based on the versioning rules.
    /// </summary>
    public bool AutoIncrementPatchNumber { get; set; } = true;

    /// <summary>
    /// Specifies the path to the Git executable that will be used by the SimpleGitVer tool.
    /// This allows for customization of the Git command-line tool location if it is not available
    /// in the system's PATH or a specific version is required.
    /// </summary>
    public string GitExe { get; set; } = "git";
}