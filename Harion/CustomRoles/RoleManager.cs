using Harion.CustomRoles.Abilities;
using Harion.Enumerations;
using Harion.HatDesigner;
using Harion.Utility.Utils;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harion.CustomRoles {

    public class RoleManager {
        public static Dictionary<PlayerControl, (Color color, string name)> specificNameInformation = new Dictionary<PlayerControl, (Color, string)>();
        public static List<RoleManager> AllRoles = new List<RoleManager>();
        public static List<PlayerControl> WinPlayer = new List<PlayerControl>();
        public List<PlayerControl> AllPlayers = new List<PlayerControl>();
        public List<PlayerControl> RoleVisibleByWhitelist = new List<PlayerControl>();
        public byte RoleId;
        public string Name = "Not Defined";
        public string TasksDescription = "Task Description is not defined go to\n your class and defined 'TasksDescription'.";
        public string IntroDescription = "Intro Description is not defined";
        public string OutroDescription = "Outro Description is not defined";
        public int NumberPlayers = 1;
        public int PercentApparition = 100;
        public bool LooseRole = false;
        public bool ForceUnshowAllRolesOnMeeting = false;
        public bool ForceExiledReveal = false;
        public bool ShowIntroCutScene = true;
        public bool TaskAreRemove = false;
        public bool IsMainRole = true;
        public bool RoleActive = true;
        public bool HasTask = true;
        public bool HasWin = false;
        public Color Color = new Color(1f, 0f, 0f, 1f);
        public IntroCutSceneTeam TeamIntro = IntroCutSceneTeam.Default;
        public RoleType RoleType = RoleType.Crewmate;
        public PlayerSide Side = PlayerSide.Crewmate;
        public VisibleBy VisibleBy = VisibleBy.Self;
        public Moment GiveTasksAt = Moment.StartGame;
        public Moment GiveRoleAt = Moment.StartGame;
        public readonly Type ClassType;

        public virtual List<Ability> Abilities { get; set; } = null;

        // Constructor
        protected RoleManager(Type type) {
            ClassType = type;
            RoleId = GetAvailableRoleId();
            AllRoles.Add(this);
            HarionPlugin.Logger.LogInfo($"Role: {type.Name} Loaded, RoleID: {RoleId}");
        }

        // Utils method
        private byte GetAvailableRoleId() {
            byte id = 0;

            while (true) {
                if (!AllRoles.Any(v => v.RoleId == id))
                    return id;

                id++;
            }
        }

        // Name Text Visibility
        internal static void PlayerNamePositon(PlayerControl Player) {
            Player.nameText.transform.localPosition = new Vector3(
                0f,
                (Player.Data.HatId == 0U) ? 2.05f :
                HatCreator.TallIds.Contains(Player.Data.HatId) ? 2.6f : 2.4f,
                -0.5f
            );
        }

        public static string NameTextVanilla(PlayerControl Player, PlayerVoteArea playerVoteArea = null) {
            if (Player == null)
                return "";

            if (!HarionPlugin.ShowRoleInName.GetValue())
                return Player.name;

            if (playerVoteArea != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding || MeetingHud.Instance.state == MeetingHud.VoteStates.Results))
                return Player.name;

            if (Player.Data.IsImpostor)
                return $"{Player.name}\nImpostor";
            else
                return $"{Player.name}\nCrewmate";
        }

        public virtual string NameText(PlayerControl Player, PlayerVoteArea playerVoteArea = null) {
            if (Player == null)
                return "";

            if (!HarionPlugin.ShowRoleInName.GetValue())
                return Player.name;

            if (playerVoteArea != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding || MeetingHud.Instance.state == MeetingHud.VoteStates.Results))
                return Player.name;

            return Player.name + "\n" + Name;
        }

        public static string NameTextSpecific(PlayerControl Player, PlayerVoteArea playerVoteArea = null) {
            KeyValuePair<PlayerControl, (Color color, string name)> SpecificPlayer = new KeyValuePair<PlayerControl, (Color color, string name)>();
            bool isContainsInSpecificList = false;

            foreach (var element in specificNameInformation) {
                if (element.Key.PlayerId == Player.PlayerId) {
                    SpecificPlayer = element;
                    isContainsInSpecificList = true;
                }
            }

            if (!isContainsInSpecificList)
                return null;

            if (Player == null)
                return "";

            if (!HarionPlugin.ShowRoleInName.GetValue())
                return Player.name;

            if (playerVoteArea != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding || MeetingHud.Instance.state == MeetingHud.VoteStates.Results))
                return Player.name;

            return Player.name + "\n" + SpecificPlayer.Value.name;
        }

        // Player List Management

        private void AddPlayer(byte PlayerId) {
            PlayerControl Player = PlayerControlUtils.FromPlayerId(PlayerId);
            if (Player != null) {
                RoleManager MainRole = GetMainRole(Player);

                if (MainRole == null || !IsMainRole) {
                    AllPlayers.Add(Player);
                    AddImportantTasks(Player);
                }
            }

            DefineVisibleByWhitelist();
        }

        public void AddPlayerRange(List<byte> PlayerIds) => PlayerIds.ForEach(p => AddPlayer(p));

        public void RpcAddPlayer(PlayerControl Player) {
            List<byte> PlayerIds = new List<byte>() { Player.PlayerId };
            AddPlayer(Player.PlayerId);

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.AddPlayer, SendOption.Reliable, -1);
            messageWriter.WriteBytesAndSize(PlayerIds.ToArray());
            messageWriter.Write(RoleId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }

        public void RpcAddPlayer(byte PlayerId) {
            List<byte> PlayerIds = new List<byte>() { PlayerId };
            AddPlayer(PlayerId);

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.AddPlayer, SendOption.Reliable, -1);
            messageWriter.WriteBytesAndSize(PlayerIds.ToArray());
            messageWriter.Write(RoleId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }

        public void RpcAddPlayerRange(List<PlayerControl> Players) {
            List<byte> PlayerIds = PlayerControlUtils.PlayerControlListToIdList(Players);
            PlayerIds.ForEach(p => AddPlayer(p));

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.AddPlayer, SendOption.Reliable, -1);
            messageWriter.WriteBytesAndSize(PlayerIds.ToArray());
            messageWriter.Write(RoleId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }

        public void RpcAddPlayerRange(List<byte> PlayerIds) {
            PlayerIds.ForEach(p => AddPlayer(p));

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.AddPlayer, SendOption.Reliable, -1);
            messageWriter.WriteBytesAndSize(PlayerIds.ToArray());
            messageWriter.Write(RoleId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }

        // Remove method
        private void RemovePlayer(byte PlayerId) {
            if (PlayerId == PlayerControl.LocalPlayer.PlayerId)
                RemoveImportantTasks(PlayerControl.LocalPlayer);

            AllPlayers.RemovePlayer(PlayerId);
            DefineVisibleByWhitelist();
        }

        public void RemovePlayerRange(List<byte> PlayerIds) => PlayerIds.ForEach(p => RemovePlayer(p));

        public void RpcRemovePlayer(PlayerControl Player) {
            List<byte> PlayerIds = new List<byte>() { Player.PlayerId };
            RemovePlayer(Player.PlayerId);

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.RemovePlayer, SendOption.Reliable, -1);
            messageWriter.WriteBytesAndSize(PlayerIds.ToArray());
            messageWriter.Write(RoleId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }

        public void RpcRemovePlayer(byte PlayerId) {
            List<byte> PlayerIds = new List<byte>() { PlayerId };
            RemovePlayer(PlayerId);

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.RemovePlayer, SendOption.Reliable, -1);
            messageWriter.WriteBytesAndSize(PlayerIds.ToArray());
            messageWriter.Write(RoleId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }

        public void RpcRemovePlayerRange(List<PlayerControl> Players) {
            List<byte> PlayerIds = PlayerControlUtils.PlayerControlListToIdList(Players);
            PlayerIds.ForEach(p => RemovePlayer(p));

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.RemovePlayer, SendOption.Reliable, -1);
            messageWriter.WriteBytesAndSize(PlayerIds.ToArray());
            messageWriter.Write(RoleId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }

        public void RpcRemovePlayerRange(List<byte> PlayerIds) {
            PlayerIds.ForEach(p => RemovePlayer(p));

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.RemovePlayer, SendOption.Reliable, -1);
            messageWriter.WriteBytesAndSize(PlayerIds.ToArray());
            messageWriter.Write(RoleId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }

        public static void SwapPlayer(byte Player1, byte Player2) {
            List<RoleManager> Player1Roles = GetAllRoles(PlayerControlUtils.FromPlayerId(Player1));
            List<RoleManager> Player2Roles = GetAllRoles(PlayerControlUtils.FromPlayerId(Player2));

            // Remove Roles for Players
            Player1Roles.ForEach(role => role.RemovePlayer(Player1));
            Player2Roles.ForEach(role => role.RemovePlayer(Player2));

            // Add Roles for Players
            Player1Roles.ForEach(role => role.AddPlayer(Player2));
            Player2Roles.ForEach(role => role.AddPlayer(Player1));
        }

        public static void RpcSwapPlayer(PlayerControl Player1, PlayerControl Player2) {
            SwapPlayer(Player1.PlayerId, Player2.PlayerId);

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SwapPlayer, SendOption.Reliable, -1);
            messageWriter.Write(Player1.PlayerId);
            messageWriter.Write(Player2.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }

        public static void RpcSwapPlayer(byte PlayerId1, byte PlayerId2) {
            SwapPlayer(PlayerId1, PlayerId2);

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SwapPlayer, SendOption.Reliable, -1);
            messageWriter.Write(PlayerId1);
            messageWriter.Write(PlayerId2);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

        }


        // Whitelist visible List
        public static List<RoleManager> GetRolesBySide(RoleType Side) => AllRoles.Where(Role => Role.RoleType == Side).ToList();

        public virtual void DefineVisibleByWhitelist() {
            RoleVisibleByWhitelist = new List<PlayerControl>();

            switch (VisibleBy) {
                case VisibleBy.Nobody:
                    RoleVisibleByWhitelist = new List<PlayerControl>();
                break;
                case VisibleBy.Self:
                    if (HasRole(PlayerControl.LocalPlayer))
                        RoleVisibleByWhitelist = new List<PlayerControl>() { PlayerControl.LocalPlayer };
                break;
                case VisibleBy.Impostor:
                    RoleVisibleByWhitelist = PlayerControl.AllPlayerControls.ToArray().Where(p => p.Data.IsImpostor).ToList();
                break;
                case VisibleBy.Crewmate:
                    RoleVisibleByWhitelist = PlayerControl.AllPlayerControls.ToArray().Where(p => !p.Data.IsImpostor).ToList();
                break;
                case VisibleBy.Everyone:
                    RoleVisibleByWhitelist = PlayerControl.AllPlayerControls.ToArray().ToList();
                break;
                case VisibleBy.Dead:
                    RoleVisibleByWhitelist = PlayerControl.AllPlayerControls.ToArray().Where(p => p.Data.IsDead).ToList();
                break;
                case VisibleBy.DeadCrewmate:
                    RoleVisibleByWhitelist = PlayerControl.AllPlayerControls.ToArray().Where(p => p.Data.IsDead && !p.Data.IsImpostor).ToList();
                break;
                case VisibleBy.DeadImpostor:
                    RoleVisibleByWhitelist = PlayerControl.AllPlayerControls.ToArray().Where(p => p.Data.IsDead && p.Data.IsImpostor).ToList();
                break;
                case VisibleBy.SameRole:
                    RoleVisibleByWhitelist = PlayerControl.AllPlayerControls.ToArray().Where(p => HasRole(p)).ToList();
                break;
            }
        }

        public static RoleManager GetRoleById(byte RoleId) {
            return AllRoles.FirstOrDefault(r => r.RoleId == RoleId);
        }

        public static bool HasMainRole(PlayerControl Player) => GetMainRole(Player) != null;

        public static bool HasRoles(PlayerControl Player) => GetAllRoles(Player).Count > 0;

        public static RoleManager GetMainRole(PlayerControl PlayerToCheck) {
            return GetAllRoles(PlayerToCheck).FirstOrDefault(role => role.IsMainRole);
        }

        public static List<RoleManager> GetAllRoles(PlayerControl PlayerToCheck) {
            List<RoleManager> listRole = new List<RoleManager>();

            foreach (var Role in AllRoles)
                foreach (var Player in Role.AllPlayers)
                    if (Player.PlayerId == PlayerToCheck.PlayerId)
                        listRole.Add(Role);

            return listRole;
        }

        public virtual void AddImportantTasks(PlayerControl Player) {
            ImportantTextTask ImportantTasks = new GameObject("RolesTasks").AddComponent<ImportantTextTask>();
            ImportantTasks.transform.SetParent(Player.transform, false);
            ImportantTasks.Text = $"{TasksDescription}{(!HasTask ? "<color=#FFFFFFFF>\nFake Tasks:</color>" : "")}";
            Player.myTasks.Insert(0, ImportantTasks);
            DestroyableSingleton<HudManager>.Instance.TaskStuff.SetActive(true);
        }

        public void RemoveImportantTasks(PlayerControl Player) {
            foreach (PlayerTask task in Player.myTasks.ToArray().ToList()) {
                if (task.name == "RolesTasks") {
                    task.OnRemove();
                    Player.myTasks.Remove(task);
                    UnityEngine.Object.Destroy(task);
                }
            }
        }

        public void RefreshTask(string newTasks, PlayerControl player) {
            RemoveImportantTasks(player);

            ImportantTextTask ImportantTasks = new GameObject("RolesTasks").AddComponent<ImportantTextTask>();
            ImportantTasks.transform.SetParent(player.transform, false);
            ImportantTasks.Text = newTasks;
            player.myTasks.Insert(0, ImportantTasks);
        }

        public bool ContainsAbility<T>() where T : Ability => Abilities.Any(s => s.Name == typeof(T).Name);

        public bool ContainsAbility(Ability ability) {
            return Abilities.Any(s => s.GetType().ToString() == ability.GetType().ToString());
        }

        public T GetAbility<T>() where T : Ability {
            if (Abilities == null || Abilities.Count == 0)
                return null;

            foreach (var ability in Abilities) {
                if (ability.Name == typeof(T).Name) {
                    return (T) ability;
                }
            }

            return null;
        }

        // Clear the list of all roles
        public static void ClearAllRoles() {
            foreach (var Role in AllRoles) {
                Role.AllPlayers.ClearPlayerList();
            }
        }

        // Has Roles
        public bool HasRole(byte PlayerId) {
            bool HasRoles = false;

            if (AllPlayers != null) {
                for (int i = 0; i < AllPlayers.Count; i++) {
                    if (PlayerId == AllPlayers[i].PlayerId)
                        HasRoles = true;
                }
            }

            return HasRoles;
        }

        public bool HasRole(PlayerControl Player) {
            bool HasRoles = false;

            if (AllPlayers != null) {
                for (int i = 0; i < AllPlayers.Count; i++) {
                    if (Player.PlayerId == AllPlayers[i].PlayerId)
                        HasRoles = true;
                }
            }

            return HasRoles;
        }

        // Operator
        public static bool operator == (RoleManager a, RoleManager b) {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.RoleId == b.RoleId;
        }

        public static bool operator != (RoleManager a, RoleManager b) {
            return !(a == b);
        }

        private bool Equals(RoleManager other) {
            return other.RoleId == RoleId;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(RoleManager))
                return false;
            return Equals((RoleManager) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(RoleId);
        }

        // Event
        public virtual void DefineIntroTeam(ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            Il2CppSystem.Collections.Generic.List<PlayerControl> newTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            switch (TeamIntro) {
                case IntroCutSceneTeam.OnlySelf:
                    newTeam.Add(PlayerControl.LocalPlayer);
                    yourTeam = newTeam;
                    break;
                case IntroCutSceneTeam.SameRole:
                    AllPlayers.ForEach(player => newTeam.Add(player));
                    break;
                case IntroCutSceneTeam.Crewmate:
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                        if (!player.Data.IsImpostor)
                            newTeam.Add(player);

                    newTeam.Add(PlayerControl.LocalPlayer);
                    break;
                case IntroCutSceneTeam.Impostor:
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                        if (player.Data.IsImpostor)
                            newTeam.Add(player);

                    newTeam.Add(PlayerControl.LocalPlayer);
                    break;
                default:
                    newTeam = yourTeam;
                    break;
            }

            yourTeam = newTeam;
        }

        public virtual void OnGameStarted() { }

        public virtual void OnGameEnded() { }

        public virtual void OnMinimapOpen(PlayerControl Player, MapBehaviour Minimap) { }

        public virtual void OnMinimapUpdate(PlayerControl Player, MapBehaviour Minimap) { }
        
        public virtual void OnMinimapClose(PlayerControl Player, MapBehaviour Minimap) { }

        public virtual void OnShipStatusStart(ShipStatus Map) { }

        public virtual void OnPlayerDisconnect(PlayerControl Player) { }

        public virtual void OnTaskComplete(PlayerControl Player) { }

        public virtual void OnTaskLeft(PlayerControl Player, int number) { }

        public virtual void OnAllTaskComplete(PlayerControl Player) { }

        public virtual void OnEnterVent(Vent vent, PlayerControl Player) { }

        public virtual void OnExitVent(Vent vent, PlayerControl Player) { }

        public virtual void OnBodyReport(PlayerControl Reporter, PlayerControl ReportBody) { }

        public virtual void OnMeetingStart(MeetingHud instance) { }

        public virtual void OnMeetingUpdate(MeetingHud instance) { }

        public virtual void OnMeetingEnd(MeetingHud instance) { }

        public virtual void OnInfectedStart() { }

        public virtual void OnInfectedEnd() { }

        public virtual void OnExiledPlayer(PlayerControl PlayerExiled) { }

        public virtual void OnLocalAttempKill(PlayerControl killer, PlayerControl target) {
            killer.RpcMurderPlayer(target);
        }

        public virtual void OnMurderKill(PlayerControl killer, PlayerControl target) { }

        public virtual void OnUpdate(PlayerControl Player) { }

        public virtual void OnLocalDie(PlayerControl Player) { }
        
        public virtual void OnPlayerDie(PlayerControl Player) { }

        public virtual void OnLocalRevive(PlayerControl Player) { }

        public virtual void OnPlayerRevive(PlayerControl Player) { }

        public virtual void OnIntroCutScene() { }

        public virtual bool OnRoleSelectedInInfected(List<PlayerControl> playerHasNoRole) {
            return false;
        }

        public virtual void OnRoleWin() { }

        // End Game Management
        public virtual bool AddEndCriteria() {
            return false;
        }

        public virtual bool WinCriteria() {
            return false;
        }

        public void RpcForceEndGame(List<PlayerControl> playersWin = null) {
            if (AmongUsClient.Instance.AmHost) {
                ForceEndGame(playersWin);
                return;
            }

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.RPCForceEndGame, SendOption.Reliable, AmongUsClient.Instance.HostId);
            messageWriter.Write(RoleId);
            messageWriter.WriteBytesAndSize(PlayerControlUtils.PlayerControlListToIdList(playersWin).ToArray());
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }

        public void ForceEndGame(List<PlayerControl> playersWin = null, bool roleLoose = true) {
            if (!AmongUsClient.Instance.AmHost)
                return;

            WinPlayer = playersWin;
            OnRoleWin();

            if (WinPlayer == null) {
                HarionPlugin.Logger.LogError("'ForceEndGame' is call, but no win players is defined, you can defined in the method argument or with override 'OnRoleWin'");
                return;
            }

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.ForceEndGame, SendOption.Reliable, -1);
            messageWriter.Write(RoleId);
            messageWriter.WriteBytesAndSize(PlayerControlUtils.PlayerControlListToIdList(WinPlayer).ToArray());
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

            // Define loses players
            var playerLoses = PlayerControl.AllPlayerControls;
            foreach (var playerLose in playerLoses.ToArray().ToList())
                foreach (var Player in WinPlayer)
                    if (playerLose.PlayerId == Player.PlayerId)
                        playerLoses.Remove(playerLose);

            // If role is loose kill player
            if (roleLoose)
                foreach (var role in AllRoles)
                    if (role.LooseRole)
                        foreach (var player in role.AllPlayers) {
                            player.Die(DeathReason.Exile);
                            player.Data.IsDead = true;
                        }

            // Set PlayerWin
            foreach (var player in WinPlayer) {
                player.Revive();
                player.Data.IsDead = false;
                player.Data.IsImpostor = true;
            }

            // Set PlayerLose
            foreach (var player in PlayerControl.AllPlayerControls) {
                player.RemoveInfected();
                player.Die(DeathReason.Exile);
                player.Data.IsDead = true;
                player.Data.IsImpostor = false;
            }

            HasWin = true;
            ShipStatus.RpcEndGame(GameOverReason.HumansByTask, false);
        }
    }
}