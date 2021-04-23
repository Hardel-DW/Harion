﻿using HardelAPI.Utility;
using HardelAPI.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HardelAPI.CustomRoles.Abilities.Kill {
    
    class KillAbility : Ability {
        public List<PlayerControl> WhiteListKill = null;
        public float KillCooldown = 0f;
        public DateTime LastKilled;
        public PlayerSide CanKill = PlayerSide.Nobody;

        public virtual void DefineKillWhiteList() {
            List<PlayerControl> AllPlayer = PlayerControl.AllPlayerControls.ToArray().ToList();

            WhiteListKill = CanKill switch
            {
                PlayerSide.Everyone => AllPlayer,
                PlayerSide.Impostor => AllPlayer.FindAll(p => p.Data.IsImpostor),
                PlayerSide.Crewmate => AllPlayer.FindAll(p => !p.Data.IsImpostor),
                PlayerSide.SameRole => AllPlayer.FindAll(p => Role.HasRole(p)),
                _ => null
            };

            if (CanKill == PlayerSide.Nobody && WhiteListKill == null && PlayerControl.LocalPlayer.Data.IsImpostor)
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
                Plugin.Logger.LogError("GetClosestTarget => WhiteListKill is null");
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
