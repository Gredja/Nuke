using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Xml;
using System.Xml.Linq;
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

    public static int Main() => Execute<Build>(x => x.Test);

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository Repository;
    [PathVariableAttribute] readonly Tool Git;

    readonly string[] projectToModify = { "ConsoleApp1", "ClassLibrary1" };


    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Log.Information("Hello world - Clean");
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(x => x.SetProjectFile(Solution));
        });

    Target Test => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Log.Information($"Hello world - Test");
            Log.Information($"Solution: {Solution}");
            Log.Information($"Rep: {Repository}");
            Log.Information($"Branch: {Repository.Branch}");
            Log.Information($"Git: {Git}");
            Log.Information($"ReleaseVersion: {ReleaseVersion}");

            Log.Information($"Create branch release/{ReleaseVersion} and checkout");
            var branchName = $"release/{ReleaseVersion}";
            Git($"branch {branchName}");
            Git($"checkout -f {branchName}");

            foreach (Project project in Solution.Projects.Where(x => projectToModify.Contains(x.Name)))
            {
                Log.Information($"project.Name: {project.Name}");
                Log.Information($"project.Path: {project.Path}");

                var doc = new XmlDocument();
                doc.Load($"{project.Path}");

                if (doc.DocumentElement != null)
                {
                    var element = doc.GetElementsByTagName("AssemblyVersion").Item(0);

                    Log.Information($"elem.Name: {element.Name}");
                    Log.Information($"elem.InnerText: {element.InnerText}");

                    element.InnerText = $"{ReleaseVersion}.0";

                    doc.Save($"{project.Path}");
                }
                else
                {
                    Log.Error("Can not load proj file!");
                }
            }

            Git("add -A ");
            Git("commit \"OK\"");
            Git("push");




        });

    Target Compile => _ => _

        .Executes(() =>
        {
            Log.Information("Hello world - Compile");

            DotNetPublish(x => x.SetProject(Solution).SetConfiguration(Configuration));
        });



}
