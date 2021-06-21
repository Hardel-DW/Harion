using System;
using UnhollowerBaseLib;

namespace Harion.Utility.Helper {
    public static class ClassHelper {

        /// <summary>
        /// Attempt to cast IL2CPP object <paramref name="obj"/> to type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type to cast to</typeparam>
        /// <param name="obj">IL2CPP object to cast</param>
        /// <param name="cast"><typeparamref name="T"/>-casted object</param>
        /// <returns>Successful cast</returns>
        public static bool TryCastTo<T>(this Il2CppObjectBase obj, out T cast) where T : Il2CppObjectBase {
            cast = obj.TryCast<T>();

            return cast != null;
        }

        /// <summary>
        /// Safely invokes event handlers by catching exceptions. Should mainly be used in game patches to prevent an exception from causing the game to hang.
        /// </summary>
        /// <typeparam name="T">Event arguments type</typeparam>
        /// <param name="eventHandler">Event to invoke</param>
        /// <param name="sender">Object invoking the event</param>
        /// <param name="args">Event arguments</param>
        public static void SafeInvoke<T>(this EventHandler<T> eventHandler, object sender, T args) where T : EventArgs {
            SafeInvoke(eventHandler, sender, args, eventHandler.GetType().Name);
        }

        /// <summary>
        /// Safely invokes event handlers by catching exceptions. Should mainly be used in game patches to prevent an exception from causing the game to hang.
        /// </summary>
        /// <typeparam name="T">Event arguments type</typeparam>
        /// <param name="eventHandler">Event to invoke</param>
        /// <param name="sender">Object invoking the event</param>
        /// <param name="args">Event arguments</param>
        /// <param name="eventName">Event name (logged in errors)</param>
        public static void SafeInvoke<T>(this EventHandler<T> eventHandler, object sender, T args, string eventName) where T : EventArgs {
            if (eventHandler == null)
                return;

            Delegate[] handlers = eventHandler.GetInvocationList();
            for (int i = 0; i < handlers.Length; i++) {
                try {
                    ((EventHandler<T>) handlers[i])?.Invoke(sender, args);
                } catch (Exception e) {
                    HarionPlugin.Logger.LogWarning($"Exception in event handler index {i} for event \"{eventName}\":\n{e}");
                }
            }
        }
    }
}
