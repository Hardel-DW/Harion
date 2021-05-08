using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using HardelAPI.CustomRoles;
using System.Linq;

namespace HardelAPI.Utility {

    public enum ClosestElement {
        Empty,
        Player,
        Vent,
        DeadBody
    }

    public class CooldownButton {
        public static bool UsableButton = true;
        public static List<CooldownButton> buttons = new List<CooldownButton>();
        public KillButtonManager killButtonManager;
        public Vector2 PositionOffset = Vector2.zero;
        public string embeddedName;
        public float MaxTimer = 0f;
        public float Timer = 0f;
        public float EffectDuration = 0f;
        public bool isEffectActive;
        public bool hasEffectDuration;

        private bool canUse;
        private Action OnClick;
        private HudManager hudManager;
        private readonly Action OnUpdate;

        #pragma warning disable IDE0052 // Supprimer les membres privés non lus
        private Color StartColorButton = new Color(255, 255, 255);
        #pragma warning restore IDE0052 // Supprimer les membres privés non lus
        private Color StartColorText = new Color(255, 255, 255);

        public Action OnEffectEnd { get; }
        public bool Enabled { get; set; } = true;
        public ClosestElement? Closest { get; set; } = null;
        public RoleManager Roles { get; set; } = null;
        public KeyCode Key { get; set; } = KeyCode.E;
        public float PixelPerUnit { get; set; }
        public bool IsDisable { get; set; } = false;
        public Sprite Sprite { get; set; }

        // Closest Element
        public GameObject closestElement { get; set; }
        public List<GameObject> allElementTargetable { get; set; } = new List<GameObject>();
        public Color ColorOutline { get; set; } = Color.white;
        public float Disntance { get; set; }

        public CooldownButton(Action OnClick, float Cooldown, string embeddedName, float pixelPerUnit, Vector2 PositionOffset, HudManager hudManager, float EffectDuration, Action OnEffectEnd = null, Action OnUpdate = null) {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.PositionOffset = PositionOffset;
            this.EffectDuration = EffectDuration;
            this.PixelPerUnit = pixelPerUnit;
            this.embeddedName = embeddedName;
            this.Sprite = HelperSprite.LoadSpriteFromEmbeddedResources(embeddedName, pixelPerUnit);

            if (OnEffectEnd != null)
                this.OnEffectEnd = OnEffectEnd;

            if (OnUpdate != null)
                this.OnUpdate = OnUpdate;

            MaxTimer = Cooldown;
            Timer = MaxTimer;
            hasEffectDuration = true;
            isEffectActive = false;
            buttons.Add(this);
            Start();
        }

        public CooldownButton(Action OnClick, float Cooldown, byte[] resource, float pixelPerUnit, Vector2 PositionOffset, HudManager hudManager, float EffectDuration, Action OnEffectEnd = null, Action OnUpdate = null) {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.PositionOffset = PositionOffset;
            this.EffectDuration = EffectDuration;
            this.PixelPerUnit = pixelPerUnit;
            this.Sprite = HelperSprite.LoadSpriteFromByte(resource, pixelPerUnit);

            if (OnEffectEnd != null)
                this.OnEffectEnd = OnEffectEnd;

            if (OnUpdate != null)
                this.OnUpdate = OnUpdate;

            MaxTimer = Cooldown;
            Timer = MaxTimer;
            hasEffectDuration = true;
            isEffectActive = false;
            buttons.Add(this);
            Start();
        }

        public CooldownButton(Action OnClick, float Cooldown, Sprite resource, float pixelPerUnit, Vector2 PositionOffset, HudManager hudManager, float EffectDuration, Action OnEffectEnd = null, Action OnUpdate = null) {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.PositionOffset = PositionOffset;
            this.EffectDuration = EffectDuration;
            this.PixelPerUnit = pixelPerUnit;
            this.Sprite = resource;

            if (OnEffectEnd != null)
                this.OnEffectEnd = OnEffectEnd;

            if (OnUpdate != null)
                this.OnUpdate = OnUpdate;

            MaxTimer = Cooldown;
            Timer = MaxTimer;
            hasEffectDuration = true;
            isEffectActive = false;
            buttons.Add(this);
            Start();
        }

        private void Start() {
            killButtonManager = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.transform);
            StartColorButton = killButtonManager.renderer.color;
            StartColorText = killButtonManager.TimerText.color;
            killButtonManager.gameObject.SetActive(true);
            killButtonManager.renderer.enabled = true;
            killButtonManager.renderer.sprite = Sprite;
            PassiveButton button = killButtonManager.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityEngine.Events.UnityAction) listener);

            void listener() {
                if (Timer < 0f && canUse && !IsDisable) {
                    killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                    if (hasEffectDuration) {
                        isEffectActive = true;
                        Timer = EffectDuration;
                        killButtonManager.TimerText.color = new Color(0, 255, 0);
                    } else {
                        Timer = MaxTimer;
                    }

                    OnClick();
                }
            }
        }

        public static void HudUpdate() {
            buttons.RemoveAll(item => item.killButtonManager == null);
            for (int i = 0; i < buttons.Count; i++) {
                if (!UsableButton)
                    buttons[i].SetCanUse(false);

                buttons[i].killButtonManager.renderer.sprite = buttons[i].Sprite;
                buttons[i].OnUpdate();
                buttons[i].Update();
            }
        }

        private void UsableUpdate() {
            bool CanUse = false;

            if (Roles.AllPlayers != null && PlayerControl.LocalPlayer != null)
                if (Roles.HasRole(PlayerControl.LocalPlayer))
                    if (!PlayerControl.LocalPlayer.Data.IsDead)
                        CanUse = !MeetingHud.Instance;

            SetCanUse(CanUse);
        }

        private void UpdateClosestElement() {
            Color outline = Roles != null ? Roles.Color : ColorOutline;

            if (closestElement != null)
                closestElement.GetComponent<SpriteRenderer>()?.material.SetFloat("_Outline", 0f);
            IsDisable = closestElement == null;

            switch (Closest) {
                case ClosestElement.DeadBody:
                break;
                case ClosestElement.Player:
                    if (allElementTargetable == null)
                        PlayerControl.AllPlayerControls.ToArray().ToList().ForEach(p => allElementTargetable.Add(p.gameObject));

                    List<PlayerControl> targets = new List<PlayerControl>();
                    allElementTargetable.ForEach(player => targets.Add(player.GetComponent<PlayerControl>()));

                    PlayerControl target = PlayerControlUtils.GetClosestPlayer(PlayerControl.LocalPlayer, targets, Disntance);
                    if (target != null)
                        closestElement = target.gameObject;
                    else
                        closestElement = null;
                    break;
                case ClosestElement.Vent:
                break;
            }

            if (closestElement != null) {
                closestElement.GetComponent<SpriteRenderer>()?.material.SetFloat("_Outline", 1f);
                closestElement.GetComponent<SpriteRenderer>()?.material.SetColor("_OutlineColor", outline);
            }
        }

        private void Update() {
            UsableUpdate();
            UpdateClosestElement();
            UpdatePosition();
            if (Timer < 0f) {
                if (IsDisable)
                    killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                else 
                    killButtonManager.renderer.color = new Color(1f, 1f, 1f, 1f);

                if (isEffectActive) {
                    killButtonManager.TimerText.color = StartColorText;
                    Timer = MaxTimer;
                    isEffectActive = false;
                    OnEffectEnd();
                }
            } else {
                if (canUse && (isEffectActive || PlayerControl.LocalPlayer.CanMove))
                    Timer -= Time.deltaTime;

                if (IsDisable)
                    killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                else
                    killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.75f);
            }
            killButtonManager.gameObject.SetActive(canUse);
            killButtonManager.renderer.enabled = canUse;
            if (canUse) {
                killButtonManager.renderer.material.SetFloat("_Desat", 0f);
                killButtonManager.SetCoolDown(Timer, MaxTimer);
            }
        }

        public void UpdatePosition() {
            if (killButtonManager.transform.localPosition.x > 0f)
                killButtonManager.transform.localPosition = new Vector3((killButtonManager.transform.localPosition.x + 1.3f) * -1, killButtonManager.transform.localPosition.y, killButtonManager.transform.localPosition.z) + new Vector3(PositionOffset.x, PositionOffset.y);
        }

        public void ForceClick(bool DoAction) {
            killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
            if (hasEffectDuration) {
                isEffectActive = true;
                Timer = EffectDuration;
                killButtonManager.TimerText.color = new Color(0, 255, 0);
            } else {
                Timer = MaxTimer;
            }

            if (DoAction)
                OnClick();
        }

        public void ForceEnd(bool DoAction) {
            Timer = 0f;
            isEffectActive = false;
            killButtonManager.TimerText.color = StartColorText;
            if (DoAction)
                OnEffectEnd();
        }

        public void Destroy() {
            UnityEngine.Object.Destroy(killButtonManager.gameObject);
        }

        public void SetCanUse(bool value) {
            this.canUse = value;
        }

        public bool GetCanUse() {
            return this.canUse;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudUpdatePatch {
        public static void Postfix() {
            CooldownButton.HudUpdate();
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public static class MeetingClosePatch {
        public static void Postfix() {
            CooldownButton.UsableButton = true;
            foreach (var button in CooldownButton.buttons) {
                button.Timer = button.MaxTimer;
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class ButtonResetPatch {
        public static void Postfix(MeetingHud __instance) {
            CooldownButton.UsableButton = false;
            for (int i = 0; i < CooldownButton.buttons.Count; i++) {
                if (CooldownButton.buttons[i].hasEffectDuration) {
                    CooldownButton.buttons[i].OnEffectEnd();
                    CooldownButton.buttons[i].isEffectActive = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class StartPatch {
        public static void Prefix(ShipStatus __instance) {
            CooldownButton.UsableButton = true;
        }
    }
}