/** This is Reactor Source Code, from js6pak.
 * If Reactor asks me, I will deleted this file.
 * 
 * Reactor Github :
 * https://github.com/NuclearPowered/Reactor/tree/master/BetterGuestPatch
 * 
 * Link to Orignal Class PlayerIdPatch :
 * https://github.com/NuclearPowered/Reactor/blob/master/Reactor/Patches/PlayerIdPatch.cs
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

using HarmonyLib;
using InnerNet;

namespace Harion.Reactor.Patch {
    internal static class BetterGuestPatch {
        [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.ChatModeType), MethodType.Getter)]
        public static class ChatModeTypePatch {
            public static bool Prefix(out QuickChatModes __result) {
                __result = QuickChatModes.FreeChatOrQuickChat;
                return false;
            }
        }
    }
}