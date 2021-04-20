using System;
using System.Collections.Generic;
using System.Reflection;

namespace HardelAPI.Utility.CustomRoles {

    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterInCustomRolesAttribute : Attribute {
        
        public RegisterInCustomRolesAttribute(Type Role) {
            ConstructorInfo ctor = Role.GetConstructor(Type.EmptyTypes);
            object Instance = ctor.Invoke(Type.EmptyTypes);
            
            PropertyInfo InstanceToSet = Instance.GetType().GetProperty("SetIntance");
            if (InstanceToSet != null) {
                InstanceToSet.SetValue(Instance, Instance);
            }
        }
    }
}
