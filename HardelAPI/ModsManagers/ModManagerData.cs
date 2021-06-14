using BepInEx;
using HardelAPI.ModsManagers.Configuration;
using System.Collections.Generic;
using UnityEngine;
using System;
using ModsSocial = HardelAPI.ModsManagers.Mods.ModsSocial;
using System.Reflection;

namespace HardelAPI.ModsManagers {
    internal class ModManagerData {

        // Global Information
        public string Name { get; set; }
        public string SmallDescription { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Credit { get; set; }

        // Auto Updater
        public string GithubRepository { get; set; }
        public string GithubAuthor { get; set; }
        public GithubVisibility GithubRepositoryVisibility { get; set; }
        public string GithubToken { get; set; }
        public string UpdateLink { get; set; }
        public bool CanUpdate { get; set; }
        public bool HasUpdate { get; set; }
        public string NewTagsVersion { get; set; }

        // Social Link
        public List<ModsSocial> SocialsLink = new List<ModsSocial>();
        public Dictionary<string, Sprite> ModsLinks { get; set; }

        // Misc
        public bool IsModActive { get; set; }
        public string AssemblyPathDirectory { get; set; }
        public string FileName { get; set; }
        public object MainClass { get; set; }
        public Type MainTypeClass { get; set; }
        public Assembly Assembly { get; set; }

        internal ModManagerData(string name, string description, string smallDescription, string version, string credit) {
            Name = name;
            Description = description;
            Version = version;
            SmallDescription = smallDescription;
            Credit = credit;
        }

        // Path
        internal string GetModPath() => $"{AssemblyPathDirectory}/{FileName}";

        // Link Builder
        internal string GithubApiLink() => $"https://api.github.com/repos/{GithubAuthor}/{GithubRepository}/releases/latest";
        internal string GithubLink() => $"https://github.com/{GithubAuthor}/{GithubRepository}";

        // Add Social Button
        internal void AddSocial() {
            SocialsLink = new List<ModsSocial>();

            if (!GithubRepository.IsNullOrWhiteSpace() && !GithubAuthor.IsNullOrWhiteSpace())
                SocialsLink.Add(new ModsSocial(GithubLink(), ModsSocial.GithubSprite));

            if (ModsLinks != null)
                foreach (var CustomLink in ModsLinks)
                    if (!CustomLink.Key.IsNullOrWhiteSpace())
                        SocialsLink.Add(new ModsSocial(CustomLink.Value, CustomLink.Key));
        }
    }
}
