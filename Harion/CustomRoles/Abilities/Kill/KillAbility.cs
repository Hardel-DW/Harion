using Harion.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harion.CustomRoles.Abilities.Kill {
    
    public class KillAbility : Ability {
        public List<PlayerControl> WhiteListKill = null;
        public float KillCooldown = 0f;
        public DateTime LastKilled;
        public Killable CanKill = Killable.Nobody;

        public virtual void DefineKillWhiteList() {
            List<PlayerControl> AllPlayer = PlayerControl.AllPlayerControls.ToArray().ToList();

            WhiteListKill = CanKill switch
            {
                Killable.Everyone => AllPlayer,
                Killable.Impostor => AllPlayer.FindAll(p => p.Data.IsImpostor),
                Killable.Crewmate => AllPlayer.FindAll(p => !p.Data.IsImpostor),
                Killable.SameRole => AllPlayer.FindAll(p => Role.HasRole(p)),
                _ => null
            };

            if (CanKill == Killable.Nobody && WhiteListKill == null && PlayerControl.LocalPlayer.Data.IsImpostor)
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
    }
}
