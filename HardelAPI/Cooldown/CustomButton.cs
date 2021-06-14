using UnityEngine;

namespace HardelAPI.Cooldown {
    public abstract class CustomButton<T> : CooldownButton where T : CustomButton<T>, new() {
        private static T instance = null;
        private static readonly object Lock = new object();
        public static T Instance {
            get {
                lock (Lock) {
                    return instance ??= new T();
                }
            }
        }
        public T SetIntance {
            set {
                instance = value;
            }
        }

        protected CustomButton() : base() { }
    }
}