using System.Linq;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Serilog;
using System.Xml;
using System.Data;

namespace Git;

public class ReleaseStrategy : Strategy, IStrategy
{
    public ReleaseStrategy(GitRepository repository, Tool git, ILogger logger) : base(repository, git, logger)
    {
    }

    public void Execute(Solution solution, string releaseVersion)
    {
        string[] projectToModify = { "ConsoleApp1", "ClassLibrary1" };

        var match = SemVerRegex.Match(releaseVersion);

        if (!match.Success)
        {
            throw new VersionNotFoundException(Repository.Branch);
        }

        Log.Information($"Create branch release/{releaseVersion} and checkout");

        var branchName = $"release/{releaseVersion}";

        Git($"branch {branchName}");
        Git($"checkout -f {branchName}");

        foreach (var project in solution.Projects.Where(x => projectToModify.Contains(x.Name)))
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

                element.InnerText = $"{releaseVersion}.0";

                doc.Save($"{project.Path}");
            }
            else
            {
                Log.Error("Can not load proj file!");
            }
        }

        var tt = "OK2";

        Git("add -A ");
        Git($"commit -m '{tt}'");
        Git($"push --set-upstream origin {branchName}");
    }
}