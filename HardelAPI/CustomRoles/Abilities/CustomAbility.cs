namespace HardelAPI.CustomRoles.Abilities {

    public abstract class Ability {
        public string Name { 
            get => GetType().Name; 
        }

        public RoleManager Role { get; set; }
    }
}
