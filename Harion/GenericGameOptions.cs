using Harion.CustomOptions;

namespace Harion {
    public static class GenericGameOptions {
        public static CustomOptionHolder HarionHeader;
        public static CustomToggleOption ShowRoleInName;
        public static CustomToggleOption DeadSeeAllRoles;
        public static CustomToggleOption ShowAllSecondaryRole;

        public static void DefineGameOptions() {
            HarionHeader = CustomOption.AddHolder("<b>Harion Option :</b>");
            ShowAllSecondaryRole = CustomOption.AddToggle("Show all role in name", false, HarionHeader);
            DeadSeeAllRoles = CustomOption.AddToggle("Dead see player role", false, HarionHeader);
            ShowRoleInName = CustomOption.AddToggle("Show role in name", false, HarionHeader);

            HarionHeader.HudStringFormat = (option, name, value) => $"\n{name}";
        }
    }
}
