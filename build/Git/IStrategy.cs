using Nuke.Common.ProjectModel;

namespace Git;

interface IStrategy
{
    public void Execute(Solution solution, string releaseVersion);
}