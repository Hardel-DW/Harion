namespace Harion.Enumerations {
    public enum VisibleBy : byte {
        Nobody = 0,
        Self,
        Impostor,
        Crewmate,
        Everyone,
        Dead,
        DeadCrewmate,
        DeadImpostor,
        SameRole
    }
}
