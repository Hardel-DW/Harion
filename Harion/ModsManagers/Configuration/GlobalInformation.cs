using System.Reflection;

namespace Harion.ModsManagers.Configuration {

    internal struct GlobalInformation {
        public string Description;
        public string Name;
        public string Version;
        public string Credit;
        public string SmallDescription;
        public bool IsActive;
        public Assembly Assembly;
    }
}
