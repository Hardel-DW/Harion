using System.Collections.Generic;
using UnityEngine;

namespace Harion.ModsManagers.Configuration {
    public interface IModManagerLink {

        public Dictionary<string, Sprite> ModsLinks { get; }

    }
}
