using Harion.Utility.Helper;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Harion.CustomRoles.FreeplayTaskTester {
    [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.Begin))]
    public static class TaskAdderPatch {

        public static void Postfix(TaskAdderGame __instance) {
            TaskFolder Template = __instance.Root.SubFolders[0];
            if (Template == null)
                return;

            TaskFolder RolesFolder = CreateFolder("Roles", __instance.Root, Template);
            CreateFolder("Crewmate", RolesFolder, Template);
            CreateFolder("Impostor", RolesFolder, Template);
            CreateFolder("Neutral", RolesFolder, Template);
            CreateFolder("Dead", RolesFolder, Template);

            __instance.GoToRoot();
            CreateScroller(__instance);
            CreateMask(__instance);
        }

        private static TaskFolder CreateFolder(string FolderName, TaskFolder Parent, TaskFolder Template) {
            List<TaskFolder> Folders = Parent.SubFolders.ToArray().ToList();

            TaskFolder RoleFolder = Object.Instantiate(Template, Template.transform.parent);
            RoleFolder.FolderName = FolderName;
            RoleFolder.name = $"Harion RoleManager - {FolderName}";
            RoleFolder.Children.Clear();
            RoleFolder.SubFolders.Clear();

            Folders.Add(RoleFolder);
            Il2CppSystem.Collections.Generic.List<TaskFolder> NewArray = new Il2CppSystem.Collections.Generic.List<TaskFolder>();
            Folders.ForEach(b => NewArray.Add(b));
            Parent.SubFolders = NewArray;

            return RoleFolder;
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
            scrollEntries.YBounds = new FloatRange(1.5f, 1.5f);
            scrollEntries.Inner = Instance.TaskParent;
            scrollEntries.enabled = false;
            ScrollerEntries.SetActive(true);
        }

        private static void CreateMask(TaskAdderGame Instance) {
            GameObject SpriteMask = new GameObject();
            SpriteMask.name = "Mask";
            SpriteMask.layer = 5;
            SpriteMask.transform.SetParent(Instance.transform);
            SpriteMask.transform.localPosition = new Vector3(0f, -0.25f, 0f);
            SpriteMask.transform.localScale = new Vector3(500f, 450f, 1f);
            SpriteMask.SetActive(true);

            SpriteMask mask = SpriteMask.AddComponent<SpriteMask>();
            mask.sprite = SpriteHelper.LoadSpriteFromEmbeddedResources("Harion.Resources.Background.png", 100f);

            BoxCollider2D collider = SpriteMask.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.enabled = true;
        }
    }
}
