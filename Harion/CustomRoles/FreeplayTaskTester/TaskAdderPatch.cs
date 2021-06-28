using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Harion.CustomRoles.FreeplayTaskTester {
    [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.Begin))]
    public static class TaskAdderPatch {

        public static void Postfix(TaskAdderGame __instance) {
            List<TaskFolder> Folders = __instance.Root.SubFolders.ToArray().ToList();
            TaskFolder Template = Folders[0];

            if (Template == null)
                return;

            TaskFolder RoleFolder = Object.Instantiate(Template, Template.transform.parent);
            RoleFolder.FolderName = "Roles";
            RoleFolder.name = "Roles Folder";
            RoleFolder.Children.Clear();
            RoleFolder.SubFolders.Clear();

            Folders.Add(RoleFolder);

            Il2CppSystem.Collections.Generic.List<TaskFolder> NewArray = new Il2CppSystem.Collections.Generic.List<TaskFolder>();
            Folders.ForEach(b => NewArray.Add(b));
            __instance.Root.SubFolders = NewArray;
            __instance.GoToRoot();

            CreateScroller(__instance);
        }

        private static void CreateScroller(TaskAdderGame Instance) {
            GameObject ScrollerEntries = new GameObject { name = "ScrollerEntries", layer = 5 };
            ScrollerEntries.transform.localPosition = new Vector3(0f, 0f, 0f);
            ScrollerEntries.transform.localScale = new Vector3(1f, 1f, 1f);
            ScrollerEntries.transform.SetParent(Instance.transform);

            Scroller scrollEntries = ScrollerEntries.AddComponent<Scroller>();
            scrollEntries.allowX = false;
            scrollEntries.allowY = true;
            scrollEntries.velocity = new Vector2(0.008f, 0.005f);
            scrollEntries.ScrollerYRange = new FloatRange(0f, 0f);
            scrollEntries.YBounds = new FloatRange(1.5f, 3f);
            scrollEntries.Inner = Instance.TaskParent;
            ScrollerEntries.SetActive(true);

        }
    }
}
