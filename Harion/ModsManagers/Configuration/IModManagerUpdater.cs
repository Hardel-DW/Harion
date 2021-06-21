namespace Harion.ModsManagers.Configuration {
    public interface IModManagerUpdater {

        string GithubRepositoryName { get; }

        string GithubAuthorName { get; }

        GithubVisibility GithubRepositoryVisibility { get; }

        string GithubAccessToken { get; }
    }
}
