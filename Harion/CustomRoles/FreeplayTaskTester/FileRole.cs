using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Harion.CustomRoles.FreeplayTaskTester {
    [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.ShowFolder))]
    public static class FileRole {

        private static List<TaskAddButton> AllFiles = new();

        public static void Postfix(TaskAdderGame __instance, [HarmonyArgument(0)] TaskFolder taskFolder) {

            if (taskFolder.Text.text == "Roles") {
                AllFiles.Clear();
                foreach (RoleManager role in RoleManager.AllRoles) {
                    TaskAddButton taskAddButton = Object.Instantiate(__instance.InfectedButton);
                    taskAddButton.Text.text = $"{role.Name}.exe";
                    taskAddButton.GetComponent<SpriteRenderer>().color = role.Color;
                    taskAddButton.GetComponent<ButtonRolloverHandler>().OutColor = role.Color;

                    taskAddButton.name = $"{role.Name}";
                    float num = 0f;
                    float num2 = 0f;
                    float num3 = 0f;
                    GetPosition(__instance, taskFolder, ref num, ref num2, ref num3);
                    AllFiles.Add(taskAddButton);

                    __instance.AddFileAsChild(taskFolder, taskAddButton, ref num, ref num2, ref num3);
                }
            }
        }

        private static void GetPosition(TaskAdderGame instance, TaskFolder folder, ref float x, ref float y, ref float z) {
            float num = 0f;
            float num2 = 0f;
            float num3 = 1.25f;

            for (int k = 0; k < folder.SubFolders.Count; k++) {
                num += instance.folderWidth;
                if (num > instance.lineWidth) {
                    num = 0f;
                    num2 -= num3;
                }
            }

            for (int k = 0; k < AllFiles.Count; k++) {
                num += instance.folderWidth;
                if (num > instance.lineWidth) {
                    num = 0f;
                    num2 -= num3;
                }
            }

            x = num;
            y = num2;
            z = num3;
        }
    }
}

