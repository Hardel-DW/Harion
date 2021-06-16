using System.Collections.Generic;
using UnityEngine;

namespace HardelAPI.ModsManagers.Configuration {
    public interface IModManagerLink {

        public Dictionary<string, Sprite> ModsLinks { get; }

    }
}
