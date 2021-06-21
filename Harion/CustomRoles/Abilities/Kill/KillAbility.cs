using Harion.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Harion.Utility.Utils;

namespace Harion.CustomRoles.Abilities.Kill {
    
    public class KillAbility : Ability {
        public List<PlayerControl> WhiteListKill = null;
        public float KillCooldown = 0f;
        public DateTime LastKilled;
        public VisibleBy CanKill = VisibleBy.Nobody;

        public virtual void DefineKillWhiteList() {
            List<PlayerControl> AllPlayer = PlayerControl.AllPlayerControls.ToArray().ToList();

            WhiteListKill = CanKill switch
            {
                VisibleBy.Everyone => AllPlayer,
                VisibleBy.Impostor => AllPlayer.FindAll(p => p.Data.IsImpostor),
                VisibleBy.Crewmate => AllPlayer.FindAll(p => !p.Data.IsImpostor),
                VisibleBy.SameRole => AllPlayer.FindAll(p => Role.HasRole(p)),
                _ => null
            };

            if (CanKill == VisibleBy.Nobody && WhiteListKill == null && PlayerControl.LocalPlayer.Data.IsImpostor)
                WhiteListKill = AllPlayer.FindAll(p => !p.Data.IsImpostor);
        }

        public float KillTimer() {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var cooldown = KillCooldown * 1000f;
            if (cooldown - (float) timeSpan.TotalMilliseconds < 0f)
                return 0;

            return (cooldown - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public PlayerControl GetClosestTarget(PlayerControl PlayerReference) {
            double distance = double.MaxValue;
            PlayerControl result = null;

            if (WhiteListKill == null) {
                HarionPlugin.Logger.LogError("GetClosestTarget => WhiteListKill is null");
            }

            foreach (var player in WhiteListKill) {
                float distanceBeetween = Vector2.Distance(player.transform.position, PlayerReference.transform.position);
                if (player.Data.IsDead || player.PlayerId == PlayerReference.PlayerId || distance < distanceBeetween)
                    continue;

                distance = distanceBeetween;
                result = player;
            }

            return result;
        }

        // Management List
        public void AddPlayerTokillWhiteList(PlayerControl Player) {
            WhiteListKill.Add(Player);
        }

        public void AddPlayerTokillWhiteList(byte PlayerId) {
            WhiteListKill.Add(PlayerControlUtils.FromPlayerId(PlayerId));
        }

        public void AddPlayerRangeTokillWhiteList(List<byte> PlayersId) {
            foreach (var PlayerId in PlayersId)
                WhiteListKill.Add(PlayerControlUtils.FromPlayerId(PlayerId));
        }

        public void AddPlayerRangeTokillWhiteList(List<PlayerControl> Players) {
            WhiteListKill.AddRange(Players);
        }

        public void RemovePlayerTokillWhiteList(byte PlayerId) {
            WhiteListKill.Remove(WhiteListKill.FirstOrDefault(p => p.PlayerId == PlayerId));
        }

        public void RemovePlayerTokillWhiteList(PlayerControl Player) {
            WhiteListKill.Remove(WhiteListKill.FirstOrDefault(p => p.PlayerId == Player.PlayerId));
        }

        public void ClearKillWhiteList() {
            WhiteListKill.Clear();
        }
    }
}
