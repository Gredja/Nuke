using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Serilog;

namespace Git;

static class StrategyFactory
{
    public static IStrategy Create(GitRepository repository, Tool git)
    {
        return new ReleaseStrategy(repository, git);
    }
}