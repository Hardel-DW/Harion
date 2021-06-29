using System;
using UnityEngine;

namespace Harion.ModsManagers {
    public class VersionUpdate {

        public string TagName { get; set; }
        public string NameVerion { get; set; }
        public string Markdown { get; set; }
        public string IdAsset { get; set; }
        public string DonwloadUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Color ButtonColor { get; set; }

        public VersionUpdate(string tagName, string nameVerion, string idAsset) {
            TagName = tagName;
            NameVerion = nameVerion;
            IdAsset = idAsset;
        }

        public static bool operator == (VersionUpdate a, VersionUpdate b) {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            return a.TagName == b.TagName;
        }

        public static bool operator != (VersionUpdate a, VersionUpdate b) {
            return !(a == b);
        }

        private bool Equals(VersionUpdate other) {
            return other.TagName == TagName;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(VersionUpdate))
                return false;
            return Equals((VersionUpdate) obj);
        }

        public override int GetHashCode() => HashCode.Combine(TagName);
    }
}
