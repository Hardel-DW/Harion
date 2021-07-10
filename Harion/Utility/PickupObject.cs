using System;
using System.Collections.Generic;
using UnityEngine;
using Harion.Utility.Utils;
using Harion.Reactor;
using UnhollowerBaseLib.Attributes;

namespace Harion.Utility {

    [RegisterInIl2Cpp]
    public class PickupObject : MonoBehaviour {

        public PickupObject(IntPtr ptr) : base(ptr) { }

        [HideFromIl2Cpp]
        public Action<PlayerControl> OnPickup { get; set; } = null;

        [HideFromIl2Cpp]
        public List<PlayerControl> PlayersCanPickup { get; set; } = new List<PlayerControl>() { PlayerControl.LocalPlayer };

        [HideFromIl2Cpp]
        public bool DestroyOnPickup { get; set; } = true;

        [HideFromIl2Cpp]
        public bool DeadCanPickup { get; set; } = false;

        void OnTriggerEnter2D(Collider2D collider) {
            PlayerControl player = collider.GetComponent<PlayerControl>();
            if (player == null || OnPickup == null)
                return;

            if (!PlayersCanPickup.ContainsPlayer(player))
                return;

            if (!DeadCanPickup && player.Data.IsDead)
                return;

            OnPickup(player);
            if (DestroyOnPickup)
                Destroy(gameObject);
        }
    }
}
