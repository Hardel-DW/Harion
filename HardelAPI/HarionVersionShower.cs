/** For VersionShower i take some code, from Reactor js6pak.
 * 
 * Reactor Github :
 * https://github.com/NuclearPowered/Reactor/tree/master/Reactor
 * 
 * Link to Orignal Class Coroutines :
 * https://github.com/NuclearPowered/Reactor/blob/master/Reactor/Patches/ReactorVersionShower.cs
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
using System.Reflection;
using BepInEx;
using BepInEx.IL2CPP;
using HardelAPI.Utility.Utils;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HardelAPI {
    public static class HarionVersionShower {
        public static TextMeshPro Text { get; private set; }

        public static event TextUpdatedHandler TextUpdated;

        public delegate void TextUpdatedHandler(TextMeshPro text);

        internal static void Initialize() {
            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) ((_, _) => {
                var original = UnityEngine.Object.FindObjectOfType<VersionShower>();
                if (!original)
                    return;

                var gameObject = new GameObject("ReactorVersion " + Guid.NewGuid());
                gameObject.transform.parent = original.transform.parent;

                var aspectPosition = gameObject.AddComponent<AspectPosition>();

                aspectPosition.Alignment = AspectPosition.EdgeAlignments.LeftTop;

                var originalAspectPosition = original.GetComponent<AspectPosition>();
                var originalPosition = originalAspectPosition.DistanceFromEdge;
                originalPosition.y = 0.15f;
                originalAspectPosition.DistanceFromEdge = originalPosition;
                originalAspectPosition.AdjustPosition();

                var position = originalPosition;
                position.x += 10.075f - 0.1f;
                position.y += 2.75f - 0.15f;
                position.z -= 1;
                aspectPosition.DistanceFromEdge = position;

                aspectPosition.AdjustPosition();

                Text = gameObject.AddComponent<TextMeshPro>();
                Text.fontSize = 2;

                UpdateText();
            }));
        }

        public static void UpdateText() {
            Text.text = "Harion " + typeof(HardelApiPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            Text.text += "\nBepInEx: " + Paths.BepInExVersion;
            Text.text += "\nMods: " + IL2CPPChainloader.Instance.Plugins.Count;
            TextUpdated?.Invoke(Text);
        }

        [HarmonyPatch(typeof(FreeWeekendShower), nameof(FreeWeekendShower.Start))]
        private static class FreeWeekendShowerPatch {
            public static bool Prefix(FreeWeekendShower __instance) {
                __instance.Output.Destroy();
                return false;
            }
        }
    }
}
