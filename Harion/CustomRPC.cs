namespace Harion {
    enum CustomRPC : byte {
        SyncroCustomGameOption = 200,
        SyncroButton,
        SetRole,

        RPCForceEndGame,
        ForceEndGame,

        PlaceCamera,
        PlaceCameraBuffer,
        SealVent,
        SealVentBuffer,
        PlaceVent,

        AddPlayer,
        RemovePlayer,
        SwapPlayer,

        Invisibility,
        FixLights,
        CleanBody,
        SetColor,
        VotingComplete,
        PickupObject
    }
}
