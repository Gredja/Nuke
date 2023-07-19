using EnsureThat;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using System.Text.RegularExpressions;

namespace Git;

public abstract class Strategy
{
    protected readonly Regex SemanticVersioningRegex = new(@"(\d+\.\d+\.\d+(?:\.\d)*)");

    protected readonly GitRepository Repository;
    protected readonly Tool Git;

    protected Strategy(GitRepository repository, Tool git)
    {
        Repository = Ensure.Any.IsNotNull(repository, nameof(repository));
        Git = Ensure.Any.IsNotNull(git, nameof(git));
    }
}