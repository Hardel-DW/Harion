using System.Collections.Generic;
using UnityEngine;

namespace HardelAPI.ModsManagers.Configuration {
    interface IModManagerLink {

        public Dictionary<string, Sprite> ModsLinks { get; }

    }
}
