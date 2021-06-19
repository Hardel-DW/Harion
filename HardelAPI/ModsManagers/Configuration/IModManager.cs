namespace HardelAPI.ModsManagers.Configuration {
    public interface IModManager {

        string DisplayName { get; }

        string Version { get; }

        string SmallDescription { get; }

        string Description { get; }
    }
}
