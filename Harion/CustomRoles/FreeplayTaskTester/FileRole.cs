using Harion.Enumerations;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Harion.Utility.Utils;
using Object = UnityEngine.Object;

namespace Harion.CustomRoles.FreeplayTaskTester {
    [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.ShowFolder))]
    public static class FileRole {

        private static List<TaskAddButton> AllFiles = new();

        public static void Postfix(TaskAdderGame __instance, [HarmonyArgument(0)] TaskFolder taskFolder) {
            if (taskFolder == null)
                return;

            AllFiles.Clear();
            string TaskFolderName = taskFolder.gameObject.name;
            if (TaskFolderName.Contains("Harion RoleManager")) {
                switch (taskFolder.Text.text) {
                    case "Impostor":
                        AddFiles(__instance, taskFolder, RoleManager.GetRolesBySide(RoleType.Impostor));
                        break;
                    case "Crewmate":
                        AddFiles(__instance, taskFolder, RoleManager.GetRolesBySide(RoleType.Crewmate));
                        break;
                    case "Neutral":
                        AddFiles(__instance, taskFolder, RoleManager.GetRolesBySide(RoleType.Neutral));
                        break;
                    case "Dead":
                        AddFiles(__instance, taskFolder, RoleManager.GetRolesBySide(RoleType.Dead));
                        break;
                }

            }

            UpdateScroll(taskFolder, __instance);
        }

        private static void UpdateScroll(TaskFolder taskFolder, TaskAdderGame __instance) {
            GameObject Scroller = __instance.gameObject.FindObject("ScrollerEntries");
            if (Scroller == null || !Scroller.scene.IsValid())
                return;

            bool Enable = taskFolder.name.Contains("Harion RoleManager");
            Scroller.GetComponent<Scroller>().enabled = Enable;
            if (!Enable)
                return;

            int row = (int) Math.Ceiling((taskFolder.SubFolders.Count + AllFiles.Count) / 5f);
            int scrollRow = Mathf.Max(row - 4, 0);

            float YRange = scrollRow != 0f ? (scrollRow * 1.25f) - 0.65f : 0f;
            Scroller.GetComponent<Scroller>().YBounds = new FloatRange(1.5f, 1.5f + YRange);
        }

        private static void Start(TaskAddButton taskFolder, RoleManager role) {
            GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
            taskFolder.Overlay.enabled = role.HasRole(data.Object);
            taskFolder.Overlay.sprite = taskFolder.CheckImage;
        }

        private static void OnClick(TaskAddButton taskFolder, RoleManager role) {
            GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
            if (role.HasRole(data.Object)) {
                role.RpcRemovePlayer(data.Object);
                taskFolder.Overlay.enabled = false;

                if (data.IsImpostor) {
                    PlayerControl.LocalPlayer.RemoveInfected();
                }
            } else {
                if (RoleManager.HasMainRole(data.Object)) {
                    RoleManager Role = RoleManager.GetMainRole(data.Object);
                    Role.RpcRemovePlayer(data.Object);
                }

                if (role.Side == PlayerSide.Impostor || role.Side == PlayerSide.DeadImpostor) {
                    PlayerControl.LocalPlayer.RpcSetInfected(new GameData.PlayerInfo[] { data });
                }

                role.RpcAddPlayer(data.Object);
                taskFolder.Overlay.enabled = true;
            }
        }

        private static void AddFiles(TaskAdderGame Instance, TaskFolder taskFolder, List<RoleManager> Roles) {
            foreach (RoleManager role in Roles) {
                TaskAddButton taskAddButton = Object.Instantiate(Instance.InfectedButton);
                taskAddButton.name = $"FileRolesHarion - {role.Name}";
                taskAddButton.Text.text = $"{role.Name}.exe";
                taskAddButton.Text.font = Resources.Load("ARIAL SDF") as TMP_FontAsset;
                taskAddButton.Text.fontMaterial = ResourceLoader.Liberia;
                taskAddButton.Overlay.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                taskAddButton.Overlay.enabled = false;
                taskAddButton.GetComponent<ButtonRolloverHandler>().OutColor = role.Color;

                SpriteRenderer renderer = taskAddButton.GetComponent<SpriteRenderer>();
                renderer.color = role.Color;
                renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                Start(taskAddButton, role);
                PassiveButton passiveButton = taskAddButton.gameObject.GetComponent<PassiveButton>();
                passiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                passiveButton.OnClick.AddListener((UnityAction) Clicked);

                // Position
                float num = 0f;
                float num2 = 0f;
                float num3 = 0f;
                GetPosition(Instance, taskFolder, ref num, ref num2, ref num3);
                AllFiles.Add(taskAddButton);

                Instance.AddFileAsChild(taskFolder, taskAddButton, ref num, ref num2, ref num3);
                void Clicked() => OnClick(taskAddButton, role);
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

