using Cake.Core.Diagnostics;
using Cake.Core.Tooling;

namespace Cake.SimpleGitVer;

public class SimpleGitVerSettings: ToolSettings
{
    public required string TagPrefix { get; set; } = string.Empty;
    public bool AutoIncrementBuildNumber { get; set; } = true;
    public string GitExe { get; set; } = "git";
}