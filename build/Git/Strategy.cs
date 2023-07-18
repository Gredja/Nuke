using EnsureThat;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Serilog;
using System.Text.RegularExpressions;

namespace Git;

public abstract class Strategy
{
    protected readonly Regex SemVerRegex = new(@"(\d+\.\d+\.\d+(?:\.\d)*)");

    protected readonly GitRepository Repository;
    protected readonly Tool Git;
    protected readonly ILogger Logger;

    protected Strategy(GitRepository repository, Tool git, ILogger logger)
    {
        Repository = Ensure.Any.IsNotNull(repository, nameof(repository));
        Git = Ensure.Any.IsNotNull(git, nameof(git));
        Logger = Ensure.Any.IsNotNull(logger, nameof(logger));
    }
}