using System.Collections.Generic;
using UnityEngine;
using Harion.CustomRoles;
using System.Linq;
using TMPro;
using Harion.Utility.Helper;
using Harion.Utility.Utils;
using Object = UnityEngine.Object;
using Hazel;
using System.Reflection;

namespace Harion.Cooldown {

    public abstract class CooldownButton {
        internal static List<CooldownButton> RegisteredButtons { get; } = new();
        public static bool UsableButton { get; internal set; } = true;
        internal int ButtonId { get; }

        // GameObject
        public KillButtonManager gameObject { get; internal set; }

        // Timer
        public float MaxTimer { get; set; } = 0f;
        public float Timer { get; set; } = 0f;
        public float EffectDuration { get; set; } = 0f;

        // Boolean
        public bool AutoDisable { get; set; } = true;
        public bool IsEffectActive { get; internal set; }
        public bool HasEffectDuration { get; set; }
        public bool Enabled { get; set; } = true;

        // Configuration
        public RoleManager Roles { get; set; } = null;
        public KeyCode Key { get; set; } = KeyCode.None;

        // Usable
        public bool CanUse { get; set; } = false;
        public bool IsDisable { get; set; } = false;

        // Visual
        public Vector2 PositionOffset { get; set; } = Vector2.zero;
        public string EmbeddedName { get; set; }
        public Sprite Sprite { get; internal set; }
        public float PixelPerUnit { get; set; } = float.MaxValue;
        public Color ColorButton { get; set; } = new Color(255, 255, 255);
        public Color DefaultColorText { get; set; } = new Color(255, 255, 255);
        public Color EffectColorText { get; set; } = new Color(0, 255, 0);

        // Number Use
        public int UseNumber { get; set; } = int.MaxValue;
        public UseNumberDecremantion DecreamteUseNimber { get; set; } = UseNumberDecremantion.Never;

        // Text Renderer
        public GameObject TextObject { get; set; }
        public TextMeshPro TextMeshPro { get; set; }

        // Closest Element
        public ClosestElement? Closest { get; set; } = null;
        public GameObject ClosestElement { get; set; } = null;
        public List<PlayerControl> AllPlayersTargetable { get; set; } = new List<PlayerControl>();
        public Color ColorOutline { get; set; } = Color.white;
        public float Disntance { get; set; } = 1f;

        // Constructor
        public CooldownButton() {
            ButtonId = GetAvailableButtonId();
            RegisteredButtons.Add(this);
        }

        internal void CreateButton(HudManager Instance) {
            HudManager HudManager = Instance;

            gameObject = Object.Instantiate(HudManager.KillButton, HudManager.transform);
            if (gameObject == null)
                HarionPlugin.Logger.LogError($"CooldownButton in MainMenuStart does not exist !");

            ColorButton = gameObject.renderer.color;
            DefaultColorText = gameObject.TimerText.color;
            gameObject.gameObject.SetActive(true);
            gameObject.renderer.enabled = true;
            gameObject.renderer.sprite = Sprite;

            PassiveButton button = gameObject.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityEngine.Events.UnityAction) Listener);

            TextObject = Object.Instantiate(gameObject.transform.GetChild(1).gameObject, gameObject.transform);
            TextObject.name = "Information Text";
            TextMeshPro = TextObject.GetComponent<TextMeshPro>();
            TextMeshPro.transform.localPosition = new Vector3(0.35f, -0.35f, 0f);

            OnCreateButton();
        }

        private void Listener() {
            if (Timer < 0f && CanUse && !IsDisable) {
                gameObject.renderer.color = new Color(ColorButton.r, ColorButton.g, ColorButton.b, 0.3f);
                if (HasEffectDuration) {
                    IsEffectActive = true;
                    Timer = EffectDuration;
                    gameObject.TimerText.color = EffectColorText;
                } else Timer = MaxTimer;

                if (DecreamteUseNimber == UseNumberDecremantion.OnClick)
                    UseNumber--;

                OnClick();
            }
        }

        internal static void HudUpdate() {
            RegisteredButtons.RemoveAll(item => item.gameObject == null);
            for (int i = 0; i < RegisteredButtons.Count; i++) {
                if (RegisteredButtons[i] == null)
                    continue;

                if (RegisteredButtons[i].gameObject == null)
                    continue;

                RegisteredButtons[i].gameObject.renderer.sprite = RegisteredButtons[i].Sprite;
                RegisteredButtons[i].Update();
            }
        }

        internal static CooldownButton GetButtonById(int id) => RegisteredButtons.FirstOrDefault(button => button.ButtonId == id);

        private void UsableUpdate() {
            bool CouldUse = true;
            bool HasRole = true;

            if (PlayerControl.LocalPlayer == null || HudManager.Instance == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (Roles != null)
                if (Roles.AllPlayers == null || PlayerControl.LocalPlayer == null)
                    HasRole = false;
                else HasRole = Roles.HasRole(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.Data.IsDead)
                CouldUse = !MeetingHud.Instance;

            if (HudManager.Instance == null)
                CouldUse = false;

            if (UseNumber <= 0)
                CouldUse = false;

            CanUse = CouldUse && HasRole;
            
            if (!UsableButton)
                CanUse = false;

            if (CanUse)
                UpdateClosestElement(CanUse);
        }

        private void UpdateClosestElement(bool CanUse) {
            if (ClosestElement != null) {
                ClosestElement.GetComponent<SpriteRenderer>()?.material.SetFloat("_Outline", 0f);
                ClosestElement = null;
            }

            if (Closest == null || Closest == Cooldown.ClosestElement.Empty)
                return;

            if (CanUse) {
                Color outline = Roles != null ? Roles.Color : ColorOutline;
                switch (Closest) {
                    case Cooldown.ClosestElement.DeadBody:
                        DeadBody targetedBody = PlayerControlUtils.GetClosestDeadBody(PlayerControl.LocalPlayer);
                        if (targetedBody != null)
                            ClosestElement = targetedBody.gameObject;

                        break;
                    case Cooldown.ClosestElement.Player:
                        if (AllPlayersTargetable == null || AllPlayersTargetable.Count == 0)
                            AllPlayersTargetable = PlayerControl.AllPlayerControls.ToArray().ToList();

                        PlayerControl targetedPlayer = PlayerControlUtils.GetClosestPlayer(PlayerControl.LocalPlayer, AllPlayersTargetable, Disntance);
                        if (targetedPlayer != null)
                            ClosestElement = targetedPlayer.gameObject;

                        break;
                    case Cooldown.ClosestElement.Vent:
                        Vent targetedVent = VentUtils.GetClosestVent(PlayerControl.LocalPlayer);
                        if (targetedVent != null)
                            ClosestElement = targetedVent.gameObject;

                        break;
                }

                if (ClosestElement != null) {
                    ClosestElement.GetComponent<SpriteRenderer>()?.material.SetFloat("_Outline", 1f);
                    ClosestElement.GetComponent<SpriteRenderer>()?.material.SetColor("_OutlineColor", outline);
                }
            }

            if (AutoDisable)
                IsDisable = ClosestElement == null;
        }

        private void UpdateText() {
            if (UseNumber != int.MaxValue && UseNumber > 0)
                SetText(UseNumber.ToString());
        }

        private void DefineButtonColor(float alpha) => gameObject.renderer.color = new Color(ColorButton.r, ColorButton.g, ColorButton.b, alpha);

        private void Update() {
            UsableUpdate();
            UpdatePosition();
            UpdateText();
            OnUpdate();

            if (Timer < 0f) {
                DefineButtonColor(IsDisable ? 0.3f : 1f);

                if (IsEffectActive) {
                    gameObject.TimerText.color = DefaultColorText;
                    Timer = MaxTimer;
                    IsEffectActive = false;

                    OnEffectEnd();

                    if (DecreamteUseNimber == UseNumberDecremantion.OnEffectEnd)
                        UseNumber--;
                } else OnCooldownEnd();
            } else {
                if (CanUse && (IsEffectActive || PlayerControl.LocalPlayer.CanMove))
                    Timer -= Time.deltaTime;

                DefineButtonColor(IsDisable ? 0.3f : 0.75f);
            }

            gameObject.gameObject.SetActive(CanUse);
            gameObject.renderer.enabled = CanUse;
            if (CanUse) {
                gameObject.renderer.material.SetFloat("_Desat", 0f);
                gameObject.SetCoolDown(Timer, MaxTimer);
            }
        }

        internal void UpdatePosition() {
            if (gameObject.transform.localPosition.x > 0f)
                gameObject.transform.localPosition = new Vector3((gameObject.transform.localPosition.x + 1.3f) * -1, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z) + new Vector3(PositionOffset.x, PositionOffset.y);
        }

        private int GetAvailableButtonId() {
            int id = 0;

            while (true) {
                if (!RegisteredButtons.Any(v => v.ButtonId == id))
                    return id;

                id++;
            }
        }

        // Public Method
        public void SetSprite(Sprite sprite) {
            Sprite = sprite;
        }

        public void SetSprite(string EmbeddedName, int PixelPerUnit) {
            Assembly assembly = Assembly.GetCallingAssembly();
            Sprite Sprite = SpriteHelper.LoadSpriteFromEmbeddedResources(EmbeddedName, PixelPerUnit, assembly);
            this.PixelPerUnit = PixelPerUnit;
            this.Sprite = Sprite;
        }

        public void SendRpc(PlayerControl Sender = null, SendOption Options = SendOption.Reliable, int TargetClientId = -1) {
            uint SenderPlayer = Sender == null ? PlayerControl.LocalPlayer.NetId : Sender.NetId;

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(SenderPlayer, (byte) CustomRPC.SyncroButton, Options, TargetClientId);
            messageWriter.Write(ButtonId);
            SendData(messageWriter);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }

        public void ForceClick(bool DoAction) {
            DefineButtonColor(0.3f);
            if (HasEffectDuration) {
                IsEffectActive = true;
                Timer = EffectDuration;
                gameObject.TimerText.color = new Color(0, 255, 0);
            } else Timer = MaxTimer;

            if (DoAction)
                OnClick();

            OnForceClick();
        }

        public void ForceEnd(bool DoAction) {
            Timer = 0f;
            IsEffectActive = false;
            gameObject.TimerText.color = DefaultColorText;
            if (DoAction)
                OnEffectEnd();

            OnForceEndEffect();
        }

        public void SetText(string text) {
            if (TextMeshPro != null)
                TextMeshPro.text = text;
            else
                HarionPlugin.Logger.LogError("TextMeshPro is not defined for this CooldownButton !");
        }

        public PlayerControl GetPlayerTarget() {
            if (Closest != Cooldown.ClosestElement.Player || ClosestElement == null)
                return null;

            return ClosestElement?.GetComponent<PlayerControl>();
        }

        public DeadBody GetDeadBodyTarget() {
            if (Closest != Cooldown.ClosestElement.DeadBody || ClosestElement == null)
                return null;

            return ClosestElement?.GetComponent<DeadBody>();
        }

        public Vent GetVentTarget() {
            if (Closest != Cooldown.ClosestElement.Vent || ClosestElement == null)
                return null;

            return ClosestElement?.GetComponent<Vent>();
        }

        // Event
        public virtual void OnCreateButton() { }

        public virtual void OnClick() { }

        public virtual void OnEffectEnd() { }

        public virtual void OnCooldownEnd() { }

        public virtual void OnUpdate() { }

        public virtual void OnPostUpdate() { }

        public virtual void OnForceClick() { }

        public virtual void OnForceEndEffect() { }

        // Rpc Automatisation
        public virtual void SendData(MessageWriter messageWriter) { }

        public virtual void ReadData(MessageReader messageReader) { }
    }
}