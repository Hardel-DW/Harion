using System;
using System.Collections.Generic;

namespace Harion.Data {
    public class DeadPlayer {
        public static List<DeadPlayer> deadPlayers = new List<DeadPlayer>();

        public PlayerControl player;
        public DateTime timeOfDeath;
        public DeathReason deathReason;
        public PlayerControl killerIfExisting;

        public DeadPlayer(PlayerControl player, DateTime timeOfDeath, DeathReason deathReason, PlayerControl killerIfExisting = null) {
            this.player = player;
            this.timeOfDeath = timeOfDeath;
            this.deathReason = deathReason;
            this.killerIfExisting = killerIfExisting;
            deadPlayers.Add(this);
        }

        public static void ClearDeadPlayer() {
            deadPlayers = new List<DeadPlayer>();
        }
    }
}
