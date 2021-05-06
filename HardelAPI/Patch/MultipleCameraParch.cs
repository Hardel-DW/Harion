using HarmonyLib;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

namespace HardelAPI.Patch {

    [HarmonyPatch]
    class SurveillanceMinigamePatch {
        private static int page = 0;
        private static float timer = 0f;
        private static List<RenderTexture> customCamera;

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Begin))]
        class SurveillanceMinigameBeginPatch {
            public static void Postfix(SurveillanceMinigame __instance) {
                page = 0;
                timer = 0;
                customCamera = new List<RenderTexture>();

                // Added Vanilla Camera to list
                for (int i = 0; i < 4; i++) {
                    PlainShipRoom plainShipRoom = __instance.FilteredRooms[i];
                    Camera camera = UnityEngine.Object.Instantiate<Camera>(__instance.CameraPrefab);
                    camera.transform.SetParent(__instance.transform);
                    camera.transform.position = plainShipRoom.transform.position + plainShipRoom.survCamera.Offset;
                    camera.orthographicSize = plainShipRoom.survCamera.CamSize;

                    RenderTexture temporary = RenderTexture.GetTemporary((int) (256f * plainShipRoom.survCamera.CamAspect), 256, 16, RenderTextureFormat.ARGB32);
                    customCamera.Add(temporary);
                    camera.targetTexture = temporary;
                }

                // Added Custom Camera to list
                if (ShipStatus.Instance.AllCameras.Length > 4 && __instance.FilteredRooms.Length > 0) {                    
                for (int i = 4; i < ShipStatus.Instance.AllCameras.Length; i++) {
                        SurvCamera surv = ShipStatus.Instance.AllCameras[i];
                        Camera camera = UnityEngine.Object.Instantiate<Camera>(__instance.CameraPrefab);
                        camera.transform.SetParent(__instance.transform);
                        camera.transform.position = new Vector3(surv.transform.position.x, surv.transform.position.y, 8f);
                        camera.orthographicSize = 2.35f;
                        
                        RenderTexture temporary = RenderTexture.GetTemporary(256, 256, 16, RenderTextureFormat.ARGB32);
                        customCamera.Add(temporary);
                        camera.targetTexture = temporary;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Update))]
        class SurveillanceMinigameUpdatePatch {

            public static bool Prefix(SurveillanceMinigame __instance) {
                timer += Time.deltaTime;
                int numberOfPages = Mathf.CeilToInt(ShipStatus.Instance.AllCameras.Length / 4f);
                bool update = false;

                // If time is elapsed or player use arrow change refresh page and set update to true.
                if (timer > 3f || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                    update = true;
                    timer = 0f;

                    if (Input.GetKeyDown(KeyCode.LeftArrow)) page = (page + numberOfPages - 1) % numberOfPages;
                    else page = (page + 1) % numberOfPages;
                }

                // If update is true, change camera.
                if ((__instance.isStatic || update) && !PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(PlayerControl.LocalPlayer)) {
                    __instance.isStatic = false;
                    for (int i = 0; i < __instance.ViewPorts.Length; i++) {
                        __instance.ViewPorts[i].sharedMaterial = __instance.DefaultMaterial;
                        __instance.SabText[i].gameObject.SetActive(false);

                        if (page * 4 + i < customCamera.Count) {
                            RenderTexture renderTexture = customCamera[page * 4 + i];
                            if (renderTexture != null)
                                __instance.ViewPorts[i].material.SetTexture("_MainTex", renderTexture);
                        } else
                            __instance.ViewPorts[i].sharedMaterial = __instance.StaticMaterial;
                    }
                } else if (!__instance.isStatic && PlayerTask.PlayerHasTaskOfType<HudOverrideTask>(PlayerControl.LocalPlayer)) {
                    __instance.isStatic = true;
                    for (int j = 0; j < __instance.ViewPorts.Length; j++) {
                        __instance.ViewPorts[j].sharedMaterial = __instance.StaticMaterial;
                        __instance.SabText[j].gameObject.SetActive(true);
                    }
                }

                return false;
            }
        }
    }
}