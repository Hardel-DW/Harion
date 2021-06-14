using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnhollowerBaseLib;

namespace HardelAPI.Utility.Utils {
    public static class SaboatageUtils {

        public static void FixSabotage() {
            SabotageSystemType system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            Il2CppArrayBase<IActivatable> specials = system.specials.ToArray();
            if (!system.dummy.IsActive | specials.Any(s => s.IsActive))
                return;

            if (ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>().IsActive)
                FixComms();
            if (ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>().IsActive)
                FixReactor(SystemTypes.Reactor);
            if (ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>().IsActive)
                FixOxygen();
            if (ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>().IsActive)
                FixAirshipReactor();
            if (ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>().IsActive)
                FixMiraComms();

            SwitchSystem fixLight = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
            if (fixLight.IsActive)
                FixLights(fixLight);
        }

        public static bool FixComms() {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 0);
            return false;
        }

        public static bool FixMiraComms() {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
            return false;
        }

        public static bool FixAirshipReactor() {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 0);
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 1);
            return false;
        }

        public static bool FixReactor(SystemTypes system) {
            ShipStatus.Instance.RpcRepairSystem(system, 16);
            return false;
        }

        public static bool FixOxygen() {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 16);
            return false;
        }

        public static bool FixLights(SwitchSystem lights) {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.FixLights, SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            lights.ActualSwitches = lights.ExpectedSwitches;
            return false;
        }
    }
}
