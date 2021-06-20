using Hazel;
using System.Linq;
using UnhollowerBaseLib;

namespace HardelAPI.Utility.Utils {
    public static class SaboatageUtils {

        public static void FixSabotages() {
            foreach (PlayerTask Task in PlayerControl.LocalPlayer.myTasks) {
                switch (Task.TaskType) {
                    case TaskTypes.ResetReactor:
                        RpcFixReactor();
                        break;
                    case TaskTypes.FixLights:
                        RpcFixLight();
                        break;
                    case TaskTypes.FixComms:
                        RpcFixMiraComms();
                        break;
                    case TaskTypes.RestoreOxy:
                        RpcFixOxygen();
                        break;
                    case TaskTypes.ResetSeismic:
                        RpcFixSeismic();
                        break;
                    case TaskTypes.StopCharles:
                        RpcFixAirshipReactor();
                        break;
                    default:
                        break;
                }
            }
        }

        public static bool SabotageActive() {
            SabotageSystemType system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            Il2CppArrayBase<IActivatable> specials = system.specials.ToArray();
            return !(!specials.Any(s => s.IsActive) | system.dummy.IsActive);
        }

        public static bool RpcFixSeismic() {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Laboratory, 16);
            return false;
        }

        public static bool RpcFixMiraComms() {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
            return false;
        }

        public static bool RpcFixAirshipReactor() {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 0);
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 1);
            return false;
        }

        public static bool RpcFixReactor() {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16);
            return false;
        }

        public static bool RpcFixOxygen() {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 0 | 64);
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 1 | 64);
            return false;
        }

        public static bool RpcFixLight() {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.FixLights, SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            FixLight();
            return false;
        }

        public static void FixLight() {
            SwitchSystem switchSystem = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
            switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
        }
    }
}
