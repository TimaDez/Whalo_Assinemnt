using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infrastructure.Networking;
using UnityEngine;

namespace Infrastructure
{
    public static class SpriteLoader
    {
        private static readonly Dictionary<string, (Texture2D tex, Sprite sprite)> _cache = new();

        public static async UniTask LoadSpriteAsync(string url, CancellationToken token = default, float ppu = 100f)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new System.ArgumentException("URL is null/empty.", nameof(url));

            if (_cache.TryGetValue(url, out var entry) && entry.sprite != null)
                return;

            var tex = await NetworkManager.GetTextureAsync(url, token);
            
            var sprite = CreateSprite(tex, ppu);
            _cache[url] = (tex, sprite);
        } 
        
        public static async UniTask<Sprite> GetSpriteAsync(string url, CancellationToken token = default, float ppu = 100f)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new System.ArgumentException("URL is null/empty.", nameof(url));

            if (_cache.TryGetValue(url, out var entry) && entry.sprite != null)
                return entry.sprite;

            var tex = await NetworkManager.GetTextureAsync(url, token);

            return GetSprite(url, tex, ppu);
        }

        private static Sprite GetSprite(string url, Texture2D tex, float ppu)
        {
            var sprite = CreateSprite(tex, ppu);
            _cache[url] = (tex, sprite);
            
            return sprite;
        }

        private static Sprite CreateSprite(Texture2D tex, float ppu)
        {
            var rect = new Rect(0, 0, tex.width, tex.height);
            var sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), ppu);
            return sprite;
        }
        
        public static bool IsCached(string url)
        {
            return !string.IsNullOrWhiteSpace(url)
                   && _cache.TryGetValue(url, out var entry)
                   && entry.sprite != null;
        }

        public static void ClearAll()
        {
            foreach (var kv in _cache)
            {
                if (kv.Value.sprite != null) Object.Destroy(kv.Value.sprite);
                if (kv.Value.tex != null) Object.Destroy(kv.Value.tex);
            }

            _cache.Clear();
        }
    }
}