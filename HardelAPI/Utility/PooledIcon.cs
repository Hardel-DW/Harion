/*using HardelAPI.Data;
using Reactor;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HardelAPI.Utility {

    [RegisterInIl2Cpp]
    public class PooledIcon : MonoBehaviour {
		private List<DangerPoint> data = new List<DangerPoint>();

		public PooledIcon(IntPtr ptr) : base(ptr) { }

        public void Show() {

        }

		public void Update() {
			for (int i = 0; i < PlayerControl.LocalPlayer.myTasks.Count; i++) {
				PlayerTask playerTask = PlayerControl.LocalPlayer.myTasks[i];
				if (playerTask.HasLocation && !playerTask.IsComplete && playerTask.LocationDirty) {
					PooledMapIcon pooledMapIcon;
					if (!this.data.TryGetValue(playerTask, out pooledMapIcon)) {
						pooledMapIcon = this.icons.Get<PooledMapIcon>();
						pooledMapIcon.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
						if (PlayerTask.TaskIsEmergency(playerTask)) {
							pooledMapIcon.rend.color = Color.red;
							pooledMapIcon.alphaPulse.enabled = true;
							pooledMapIcon.rend.material.SetFloat("_Outline", 1f);
						} else {
							pooledMapIcon.rend.color = Color.yellow;
						}
						this.data.Add(playerTask, pooledMapIcon);
					}
					MapTaskOverlay.SetIconLocation(playerTask, pooledMapIcon);
				}
			}
		}

		public void Hide() {
			foreach (var point in data)


			this.data.Clear();
			base.gameObject.SetActive(false);
		}
	}
}
*/