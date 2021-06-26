using HarmonyLib;
using System;
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
        }
    }
}
