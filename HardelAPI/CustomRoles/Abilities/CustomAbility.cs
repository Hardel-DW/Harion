namespace HardelAPI.CustomRoles.Abilities {

    public abstract class Ability {
        public string Name { 
            get => GetType().ToString(); 
        }

        public RoleManager Role { get; set; }
    }
}
