using UnityEngine;

namespace Harion.CustomRoles {
    public sealed class SpecificData {

        public PlayerControl Player { get; }
        public Color Color { get; }
        public string Name { get; }
        public PriorityRenderer PriorityRenderer { get; }

        public SpecificData(PlayerControl player, Color color, string name, PriorityRenderer priorityRenderer = PriorityRenderer.Normal) {
            Player = player;
            Color = color;
            Name = name;
            PriorityRenderer = priorityRenderer;
        }
    }

    public enum PriorityRenderer : int {
        Last = 0,
        VeryLow = 100,
        Low = 200,
        Normal = 300,
        High = 400,
        VeryHigh = 500,
        First = 600,
        OverrideEverything = int.MaxValue
    }
}
