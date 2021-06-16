using HardelAPI.Utility.Helper;
using HardelAPI.Utility.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace HardelAPI.ModsManagers.Mods {
    public class ModsSocial {

        public Sprite Icone;
        public string Link;
        
        public ModsSocial(Sprite Icone, string Link) {
            this.Icone = Icone;
            this.Link = Link;
        }

        public ModsSocial(string Link, Sprite Icone) {
            this.Icone = Icone;
            this.Link = Link;
        }

        // Social Sprite
        public static Sprite YoutubeSprite => SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Social.Youtube.png", 100f).DontDestroy();
        public static Sprite TwitchSprite => SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Social.Twitch.png", 100f).DontDestroy();
        public static Sprite PatreonSprite => SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Social.Patreon.png", 100f).DontDestroy();
        public static Sprite PaypalSprite => SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Social.Paypal.png", 100f).DontDestroy();
        public static Sprite DiscordSprite => SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Social.Discord.png", 100f).DontDestroy();
        public static Sprite GithubSprite => SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Social.Github.png", 100f).DontDestroy();

        // Create Game Object
        internal GameObject CreateObject(Vector3 Position, GameObject Parent) {
            GameObject SocialLink = new GameObject { name = "Link", layer = 1 };
            SocialLink.transform.SetParent(Parent.transform);
            SocialLink.transform.localPosition = Position;
            SocialLink.transform.localScale = new Vector2(0.1f, 0.1f);

            BoxCollider2D collider = SocialLink.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(5f, 5f);
            
            PassiveButton button = SocialLink.AddComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityAction) OnClick);
            button.OnMouseOver = new UnityEvent();
            button.OnMouseOver.AddListener((UnityAction) OnMouseOver);
            button.OnMouseOut = new UnityEvent();
            button.OnMouseOut.AddListener((UnityAction) OnMouseOut);

            SpriteRenderer renderer = SocialLink.AddComponent<SpriteRenderer>();
            renderer.sprite = Icone;
            renderer.size = new Vector2(1f, 1f);
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            SocialLink.SetActive(true);
            void OnClick() => PopupMessage.PopupLink($"Are you sure you want to continue on the following link?\n{Link}", Link);
            void OnMouseOver() => SocialLink.GetComponent<SpriteRenderer>().color = new Color(0.3f, 1f, 0.3f, 1f);
            void OnMouseOut() => SocialLink.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1, 1f);

            return SocialLink;
        }
    }
}
