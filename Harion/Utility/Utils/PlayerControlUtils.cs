using Harion.Enumerations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harion.Utility.Utils {
    public static class PlayerControlUtils {
        public static PlayerControl FromNetId(uint netId) {
            foreach (var player in PlayerControl.AllPlayerControls)
                if (player.NetId == netId)
                    return player;

            return null;
        }

        public static PlayerControl FromPlayerId(byte id) {
            for (int i = 0; i < PlayerControl.AllPlayerControls.Count; i++)
                if (PlayerControl.AllPlayerControls[i].PlayerId == id)
                    return PlayerControl.AllPlayerControls[i];

            return null;
        }

        public static List<byte> PlayerControlListToIdList(List<PlayerControl> players) {
            List<byte> playerIds = new List<byte>();

            foreach (var player in players)
                playerIds.Add(player.PlayerId);

            return playerIds;
        }

        public static List<PlayerControl> IdListToPlayerControlList(List<byte> playerIds) {
            List<PlayerControl> players = new List<PlayerControl>();

            foreach (var playerId in playerIds)
                players.Add(FromPlayerId(playerId));

            return players;
        }

        public static IEnumerator FlashCoroutine(Color color, float waitfor = 1f, float alpha = 0.3f) {
            color.a = alpha;
            var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
            var oldcolour = fullscreen.color;
            fullscreen.enabled = true;
            fullscreen.color = color;
            yield return new WaitForSeconds(waitfor);
            fullscreen.enabled = false;
            fullscreen.color = oldcolour;
        }

        public static void Telportation(Vector2 position, PlayerControl player) {
            player.NetTransform.RpcSnapTo(position);
        }

        public static void KillPlayerArea(Vector2 psotion, PlayerControl murder, float size) {
            foreach (var player in PlayerControl.AllPlayerControls) {
                if (player.PlayerId == murder.PlayerId)
                    continue;

                float distance = Vector2.Distance(psotion, player.GetTruePosition());
                if (distance < size)
                    player.RpcMurderPlayer(player);
            }
        }

        public static void KillEveryone(PlayerControl murder) {
            foreach (var player in PlayerControl.AllPlayerControls) {
                if (player.PlayerId == murder.PlayerId)
                    continue;

                player.MurderPlayer(player);
            }
        }

        public static bool AmHost() {
            return AmongUsClient.Instance.AmHost;
        }

        public static PlayerControl GetClosestPlayer(PlayerControl PlayerReference) {
            double distance = double.MaxValue;
            PlayerControl result = null;

            foreach (var player in PlayerControl.AllPlayerControls) {
                float distanceBeetween = Vector2.Distance(player.transform.position, PlayerReference.transform.position);
                if (player.Data.IsDead || player.PlayerId == PlayerReference.PlayerId || distance < distanceBeetween)
                    continue;

                distance = distanceBeetween;
                result = player;
            }

            return result;
        }

        public static PlayerControl GetClosestPlayer(PlayerControl PlayerReference, List<PlayerControl> whitelist, float maxRange = 5f) {
            double distance = double.MaxValue;
            PlayerControl result = null;

            foreach (var player in whitelist) {
                float distanceBeetween = Vector2.Distance(player.transform.position, PlayerReference.transform.position);
                if (player.Data.IsDead || player.PlayerId == PlayerReference.PlayerId || distance < distanceBeetween)
                    continue;

                distance = distanceBeetween;
                result = player;
            }

            if (distance > maxRange)
                return null;

            return result;
        }

        public static DeadBody GetClosestDeadBody(PlayerControl PlayerReference, float maxRange = 1f) {
            double distance = double.MaxValue;
            DeadBody result = null;

            foreach (var dead in Object.FindObjectsOfType<DeadBody>()) {
                PlayerControl playerFromDEead = FromPlayerId(dead.ParentId);

                float distanceBeetween = Vector2.Distance(dead.transform.position, PlayerReference.transform.position);
                if (!playerFromDEead.Data.IsDead || playerFromDEead.PlayerId == PlayerReference.PlayerId || distance < distanceBeetween)
                    continue;

                distance = distanceBeetween;
                result = dead;
            }

            if (distance > maxRange)
                return null;

            return result;
        }

        /// <summary>
        /// Player speed
        /// </summary>
        public static void Speed(PlayerControl Player, float Speed) {
            Player.MyPhysics.Speed = Speed;
        }

        public static void ClearPlayerList(this List<PlayerControl> list) => list.Clear();

        public static bool ContainsPlayer(this List<PlayerControl> list, PlayerControl player) => ContainsPlayer(list, player.PlayerId);

        public static bool ContainsPlayer(this List<PlayerControl> list, byte PlayerId) => list.FirstOrDefault(p => p.PlayerId == PlayerId);

        public static void AddPlayer(this List<PlayerControl> list, PlayerControl Player) => list.Add(Player);

        public static void AddPlayer(this List<PlayerControl> list, byte PlayerId) => list.Add(FromPlayerId(PlayerId));

        public static void AddPlayerRange(this List<PlayerControl> list, List<PlayerControl> Players) => list.AddRange(Players);

        public static void AddPlayerRange(this List<PlayerControl> list, List<byte> PlayersId) {
            foreach (var PlayerId in PlayersId)
                list.Add(FromPlayerId(PlayerId));
        }

        public static void RemovePlayer(this List<PlayerControl> list, byte PlayerId) {
            PlayerControl exist = list.FirstOrDefault(p => p.PlayerId == PlayerId);

            if (exist != null)
                list.Remove(list.FirstOrDefault(p => p.PlayerId == PlayerId));
        }

        public static void RemovePlayer(this List<PlayerControl> list, PlayerControl Player) {
            PlayerControl exist = list.FirstOrDefault(p => p.PlayerId == Player.PlayerId);

            if (exist != null)
                list.Remove(list.FirstOrDefault(p => p.PlayerId == Player.PlayerId));
        }


        // Dictionary
        public static void ClearPlayerList<T>(this Dictionary<PlayerControl, T> list) => list.Clear();

        public static bool ContainsPlayer<T>(this Dictionary<PlayerControl, T> list, PlayerControl player) => list.ContainsPlayer(player.PlayerId);

        public static bool ContainsPlayer<T>(this Dictionary<PlayerControl, T> list, byte PlayerId) => list.Any(p => p.Key.PlayerId == PlayerId);

        public static void RemovePlayer<T>(this Dictionary<PlayerControl, T> list, byte PlayerId) {
            PlayerControl PlayerToRemove = list.FirstOrDefault(p => p.Key.PlayerId == PlayerId).Key;

            if (PlayerToRemove != null)
                list.Remove(PlayerToRemove);
        }

        public static void RemovePlayer<T>(this Dictionary<PlayerControl, T> list, PlayerControl Player) {
            if (Player != null && list != null)
                list.RemovePlayer(Player);
        }

        public static void UpdatePlayerValue<T>(this Dictionary<PlayerControl, T> list, byte PlayerId, T value) {
            PlayerControl Player = list.FirstOrDefault(p => p.Key.PlayerId == PlayerId).Key;
            list[Player] = value;
        }

        public static void UpdatePlayerValue<T>(this Dictionary<PlayerControl, T> list, PlayerControl Player, T value) {
            if (Player != null && list != null)
                list.UpdatePlayerValue(Player, value);
        }

        public static void AddOrUpdatePlayerValue<T>(this Dictionary<PlayerControl, T> list, byte PlayerId, T value) {
            PlayerControl Player = list.FirstOrDefault(p => p.Key.PlayerId == PlayerId).Key;

            if (!list.ContainsPlayer(Player))
                list.Add(Player, value);
            else
                list.UpdatePlayerValue(Player, value);
        }

        public static void AddOrUpdatePlayerValue<T>(this Dictionary<PlayerControl, T> list, PlayerControl Player, T value) {
            if (Player != null && list != null)
                list.AddOrUpdatePlayerValue(Player.PlayerId, value);
        }

        public static T GetValueFromPlayer<T>(this Dictionary<PlayerControl, T> list, PlayerControl player) => list.GetValueFromPlayer(player.PlayerId);

        public static T GetValueFromPlayer<T>(this Dictionary<PlayerControl, T> list, byte PlayerId) => list.FirstOrDefault(p => p.Key.PlayerId == PlayerId).Value;

        public static Color32 ToPlayerColor(this ColorType color) => Palette.PlayerColors[(byte) color];

        public static Color32 ToShadowColor(this ColorType color) => Palette.ShadowColors[(byte) color];

        public static List<PlayerControl> GetImpostors() => PlayerControl.AllPlayerControls.ToArray().Where(p => p.Data.IsImpostor).ToList();
        public static List<PlayerControl> GetCrewmate() => PlayerControl.AllPlayerControls.ToArray().Where(p => !p.Data.IsImpostor).ToList();
        public static List<PlayerControl> GetEveryone() => PlayerControl.AllPlayerControls.ToArray().ToList();
        public static List<PlayerControl> GetDead() => PlayerControl.AllPlayerControls.ToArray().Where(p => p.Data.IsDead).ToList();
        public static List<PlayerControl> GetCrewamteDead() => PlayerControl.AllPlayerControls.ToArray().Where(p => p.Data.IsDead && !p.Data.IsImpostor).ToList();
        public static List<PlayerControl> GetImpostorDead() => PlayerControl.AllPlayerControls.ToArray().Where(p => p.Data.IsDead && p.Data.IsImpostor).ToList();

        public static bool IsPlayerNull => PlayerControl.AllPlayerControls == null || PlayerControl.AllPlayerControls.Count <= 0 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null;
    }
}
