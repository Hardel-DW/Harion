using UnityEngine;
using Harion.Utility.Helper;
using System.Collections.Generic;
using BepInEx.Configuration;
using Harion.Utility.Utils;

namespace Harion.CustomKeyBinds {
    public class CustomKeyBind {
        public static Dictionary<string, List<CustomKeyBind>> KeyBinds = new Dictionary<string, List<CustomKeyBind>>();
        public KeyCode Key;
        public readonly KeyCode DefautKey;
        public readonly string ModId;
        public readonly string Name;
        public readonly string Section;
        private ConfigEntry<string> Entry;

        public CustomKeyBind(KeyCode defautKey, string name, string section) {
            DefautKey = defautKey;
            Key = defautKey;
            Name = name;
            Section = section;
            ModId = PluginHelper.GetCallingPluginId() == "" ? "Vanilla" : PluginHelper.GetCallingPluginId();
            Entry = HarionPlugin.Instance.Config.Bind($"{ModId}-Keybind", Name, Key.ToString());
            Key = KeyCodeUtils.KeyCodeFromString(Entry.Value);
            AddToDictionary();
        }

        public void SetKey(KeyCode Key) {
            this.Key = Key;
            Entry.Value = Key.ToString();
        }

        private void AddToDictionary() {
            bool IsFound = false;
            foreach (KeyValuePair<string, List<CustomKeyBind>> KeyBind in KeyBinds) {
                if (KeyBind.Key == Section) {
                    KeyBind.Value.Add(this);
                    IsFound = true;
                }
            }

            if (!IsFound)
                KeyBinds.Add(Section, new List<CustomKeyBind>() { this });
        }

        public static CustomKeyBind AddCustomKeyBind(KeyCode defautKey, string name, string section) => new CustomKeyBind(defautKey, name, section);
    }
}
