/** This is Reactor Source Code, from js6pak.
 * If Reactor asks me, I will deleted this file.
 * 
 * Reactor Github :
 * https://github.com/NuclearPowered/Reactor/tree/master/Reactor
 * 
 * Link to Orignal Class PluginSingleton :
 * https://github.com/NuclearPowered/Reactor/blob/master/Reactor/PluginSingleton.cs 
 * 
 * Discord :
 * https://discord.gg/et5XGTMfPz
 * 
 * Website :
 * https://reactor.gg/
 * 
 * Documentation :
 * https://docs.reactor.gg/
*/

using System.Linq;
using System;
using BepInEx.IL2CPP;

namespace HardelAPI.Reactor {
    public static class PluginSingleton<T> where T : BasePlugin {
        private static T _instance;

        public static T Instance {
            get => _instance ??= IL2CPPChainloader.Instance.Plugins.Values.Select(x => x.Instance).OfType<T>().Single();

            set {
                if (_instance != null)
                    throw new Exception($"Instance for {typeof(T).FullName} is already set");

                _instance = value;
            }
        }
    }
}