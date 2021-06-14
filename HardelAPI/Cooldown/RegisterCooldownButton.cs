using System;
using System.Reflection;

namespace HardelAPI.Cooldown {

    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterCooldownButton : Attribute {

        public static void Register() {
            Register(Assembly.GetCallingAssembly());
        }

        public static void Register(Assembly assembly) {
            foreach (var type in assembly.GetTypes()) {
                RegisterCooldownButton attribute = type.GetCustomAttribute<RegisterCooldownButton>();

                if (attribute != null) {
                    if (!type.IsSubclassOf(typeof(CooldownButton)))
                        throw new InvalidOperationException($"Type {type.Name} must extend {nameof(CooldownButton)}.");

                    ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
                    object Instance = ctor.Invoke(Type.EmptyTypes);

                    PropertyInfo InstanceToSet = Instance.GetType().GetProperty("SetIntance");
                    if (InstanceToSet != null)
                        InstanceToSet.SetValue(Instance, Instance);
                }
            }
        }
    }
}
