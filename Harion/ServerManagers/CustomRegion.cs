using BepInEx;
using BepInEx.Configuration;
using Harion.ServerManagers.Patch;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Harion.ServerManagers {
    public static class CustomRegion {

        public static IRegionInfo AddRegion(string name, string ip, ushort port) {
            if (Uri.CheckHostName(ip) != UriHostNameType.IPv4)
                return ServerManager.Instance.CurrentRegion;

            IRegionInfo existingRegion = ServerManager.DefaultRegions.ToArray().FirstOrDefault(region => region.PingServer == ip);
            if (existingRegion != null)
                return existingRegion;

            IRegionInfo newRegion = new DnsRegionInfo(ip, name, StringNames.NoTranslation, ip, port).Cast<IRegionInfo>();
            RegionsPatch.ModRegions.Add(newRegion);
            RegionsPatch.Patch();

            return newRegion;
        }

        public static bool SetDirectRegion(string ip, out IRegionInfo newRegion) {
            newRegion = null;

            if (!Regex.IsMatch(ip, @"^(\d{1,3}\.){3}\d{1,3}$"))
                return false;

            newRegion = new DnsRegionInfo(ip, ip, StringNames.NoTranslation, ip, 22023).Cast<IRegionInfo>();

            RegionsPatch.DirectRegion = newRegion;
            RegionsPatch.Patch();

            return true;
        }
    }
}
