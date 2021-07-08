using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System.Linq;
using TMPro;
using Harion.Utility.Helper;
using Harion.Cooldown;

namespace Harion.Utility {
    public class PlayerButton {
        public static List<PlayerButton> buttons = new List<PlayerButton>();
        public static Action OnClose;
        public KillButtonManager killButtonManager;
        public Vector2 PositionOffset = Vector2.zero;
        public Action OnClick;
        private HudManager hudManager;
        private SpriteRenderer herePoint;
        private GameObject TextPlaynerName;
        private TextMeshPro TMPPlaynerName;
        private PlayerControl Player;
        private List<PlayerControl> BlackList = new List<PlayerControl>();
        private bool showDead;

        public PlayerButton(Action OnClick, PlayerControl Player, Vector2 PositionOffset, HudManager hudManager, bool showDead, List<PlayerControl> BlackList) {
            herePoint = UnityEngine.Object.Instantiate(MapBehaviour.Instance.HerePoint, hudManager.transform);
            Player.SetPlayerMaterialColors(herePoint);
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.PositionOffset = PositionOffset;
            this.Player = Player;
            this.showDead = showDead;
            this.BlackList = BlackList;

            buttons.Add(this);
            Start();
        }

        public PlayerButton(Action OnClick, PlayerControl Player, Vector2 PositionOffset, HudManager hudManager, bool showDead) {
            herePoint = UnityEngine.Object.Instantiate(MapBehaviour.Instance.HerePoint, hudManager.transform);
            Player.SetPlayerMaterialColors(herePoint);
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.PositionOffset = PositionOffset;
            this.Player = Player;
            this.showDead = showDead;
            buttons.Add(this);
            Start();
        }

        private void Start() {
            // Create Kill Button
            killButtonManager = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.transform);
            killButtonManager.gameObject.SetActive(true);
            killButtonManager.TimerText.enabled = false;
            UnityEngine.Object.Destroy(killButtonManager.GetComponent<SpriteRenderer>());
            UnityEngine.Object.Destroy(killButtonManager.killText);
            PassiveButton button = killButtonManager.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityEngine.Events.UnityAction) listner);
            void listner() {
                if ((!showDead && Player.Data.IsDead) || BlackList.Any(item => item.PlayerId == Player.PlayerId))
                    return;

                StopPlayerSelection();
                OnClick();
            }

            if (killButtonManager.transform.localPosition.x > 0f)
                killButtonManager.transform.localPosition = new Vector3((killButtonManager.transform.localPosition.x + 1.3f) * -1, killButtonManager.transform.localPosition.y, killButtonManager.transform.localPosition.z) + new Vector3(PositionOffset.x, PositionOffset.y);

            // Create Text for Button.
            TextPlaynerName = killButtonManager.gameObject.CreateTMP(Player.Data.PlayerName, new Vector2(0, -0.4f), Palette.PlayerColors[Player.Data.ColorId], 2);
            TMPPlaynerName = TextPlaynerName.GetComponent<TextMeshPro>();
            TMPPlaynerName.alignment = TextAlignmentOptions.Center;

            // PlayerIcon
            herePoint.gameObject.transform.SetParent(killButtonManager.transform);
            herePoint.transform.localPosition = Vector2.zero;
            herePoint.transform.localScale = new Vector2(2.5f, 2.5f);

            // If Player is dead, and you don't want to show dead.
            if ((!showDead && Player.Data.IsDead) || BlackList.Any(item => item.PlayerId == Player.PlayerId)) {
                TMPPlaynerName.color = new Color(TMPPlaynerName.color.r, TMPPlaynerName.color.g, TMPPlaynerName.color.b, 0.3f);
                herePoint.color = new Color(herePoint.color.r, herePoint.color.g, herePoint.color.b, 0.3f);
            }
        }

        public static void HudUpdate() {
            buttons.RemoveAll(item => item.killButtonManager == null);
            for (int i = 0; i < buttons.Count; i++) {
                buttons[i].Update();
            }
        }

        private void Update() {
            if (killButtonManager.transform.localPosition.x > 0f)
                killButtonManager.transform.localPosition = new Vector3((killButtonManager.transform.localPosition.x + 1.3f) * -1, killButtonManager.transform.localPosition.y, killButtonManager.transform.localPosition.z) + new Vector3(PositionOffset.x, PositionOffset.y);

            float alpha = 0;
            if ((!showDead && Player.Data.IsDead) || BlackList.Any(item => item.PlayerId == Player.PlayerId)) alpha = 0.3f;
            else alpha = 1f;

            TMPPlaynerName.color = new Color(TMPPlaynerName.color.r, TMPPlaynerName.color.g, TMPPlaynerName.color.b, alpha);
            herePoint.color = new Color(herePoint.color.r, herePoint.color.g, herePoint.color.b, alpha);

            if (Input.GetKeyDown(KeyCode.Escape)) {
                StopPlayerSelection();
                OnClose();
            }
        }

        public static void UpdateBlacklist(List<PlayerControl> BlackList) {
            if (buttons != null && buttons.Count > 0)
                for (int i = 0; i < buttons.Count; i++)
                    buttons[i].BlackList = BlackList;
        }

        public static void StopPlayerSelection() {
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].Destroy();

            for (int i = 0; i < CooldownButton.RegisteredButtons.Count; i++)
                CooldownButton.RegisteredButtons[i].UpdatePosition();

            CooldownButton.UsableButton = true;
        }

        public void Destroy() {
            UnityEngine.Object.Destroy(killButtonManager.gameObject);
            buttons.RemoveAll(item => item.killButtonManager == null);
        }

        public static void CheckEspace(Action onClose) {
            OnClose = onClose;
        }

        public static void InitPlayerButton(bool showDead, List<PlayerControl> BlackList, Action<PlayerControl> action, Action OnClose) {
            CooldownButton.UsableButton = false;
            DestroyableSingleton<HudManager>.Instance.ShowMap((Action<MapBehaviour>) (map => {
                map.gameObject.SetActive(false);
                map.HerePoint.enabled = true;
            }));

            CheckEspace(() => OnClose());

            for (int i = 0; i < PlayerControl.AllPlayerControls.Count; i++) {
                PlayerControl currentPlayer = PlayerControl.AllPlayerControls[i];

                float row = 3;
                float x = (i % row);
                float y = ((i - (i % row)) / row);

                PlayerButton button = null;
                button = new PlayerButton(
                    () => action(currentPlayer),
                    currentPlayer,
                    new Vector2(x, y),
                    HudManager.Instance,
                    showDead,
                    BlackList
                );
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Awake))]
    public static class PlayerButtonResetMeetingHudPatch {
        public static void Postfix() {
            PlayerButton.StopPlayerSelection();
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudUpdatePlayerPatch {
        public static void Postfix() {
            PlayerButton.HudUpdate();
        }
    }
}