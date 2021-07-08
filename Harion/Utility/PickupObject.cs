using System;
using System.Collections.Generic;
using UnityEngine;
using Harion.Utility.Utils;
using Harion.Reactor;
using UnhollowerBaseLib.Attributes;
using InnerNet;

namespace Harion.Utility {

    [RegisterInIl2Cpp]
    public class PickupObject : InnerNetObject {

        [HideFromIl2Cpp]
        public Action OnPickup { get; set; } = null;

        [HideFromIl2Cpp]
        public List<PlayerControl> PlayersCanPickup { get; set; } = new List<PlayerControl>() { PlayerControl.LocalPlayer };

        [HideFromIl2Cpp]
        public bool DestroyOnPickup { get; set; } = true;

        [HideFromIl2Cpp]
        public bool DeadCanPickup { get; set; } = false;

        public PickupObject(IntPtr ptr) : base(ptr) { }

        void OnTriggerEnter2D(Collider2D collider) {
            PlayerControl player = collider.GetComponent<PlayerControl>();
            if (player == null || OnPickup == null)
                return;

            if (!PlayersCanPickup.ContainsPlayer(player))
                return;

            if (!DeadCanPickup && player.Data.IsDead)
                return;

            OnPickup();
            if (DestroyOnPickup)
                Destroy(gameObject);
        }
    }
}
