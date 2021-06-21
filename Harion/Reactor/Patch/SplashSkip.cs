/** This is Reactor Source Code, from js6pak.
 * If Reactor asks me, I will deleted this file.
 * 
 * Reactor Github :
 * https://github.com/NuclearPowered/Reactor/tree/master/Reactor
 * 
 * Link to Orignal Class SplashSkip :
 * https://github.com/NuclearPowered/Reactor/blob/master/Reactor/Patches/SplashSkip.cs
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

using System;
using UnityEngine.SceneManagement;

namespace Harion.Reactor.Patch {
    internal static class SplashSkip {
        internal static void Initialize() {
            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) ((scene, _) => {
                if (scene.name == "SplashIntro") {
                    SceneManager.LoadScene("MainMenu");
                }
            }));
        }
    }
}
