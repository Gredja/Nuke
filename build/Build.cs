using Git;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("ReleaseVersion")]
    readonly string ReleaseVersion;

    public static int Main() => Execute<Build>(x => x.Release);

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository Repository;
    [PathVariableAttribute] readonly Tool Git;

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(x => x.SetProjectFile(Solution));
        });

    Target Release => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Log.Information("Test");
            Log.Information($"Solution: {Solution}");
            Log.Information($"Rep: {Repository}");
            Log.Information($"Branch: {Repository.Branch}");
            Log.Information($"Git: {Git}");
            Log.Information($"ReleaseVersion: {ReleaseVersion}");

            StrategyFactory.Create(Repository, Git).Execute(Solution, ReleaseVersion);
        });
}
