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

        public static async UniTask<Sprite> GetSpriteAsync(string driveLink, CancellationToken token = default, float ppu = 100f)
        {
            if (string.IsNullOrWhiteSpace(driveLink))
                throw new System.ArgumentException("driveLink is null/empty.", nameof(driveLink));

            if (_cache.TryGetValue(driveLink, out var entry) && entry.sprite != null)
                return entry.sprite;

            var tex = await NetworkManager.GetTextureAsync(driveLink, token);

            var rect = new Rect(0, 0, tex.width, tex.height);
            var sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), ppu);
            _cache[driveLink] = (tex, sprite);
            return sprite;
        }

        public static bool IsCached(string driveLink)
        {
            return !string.IsNullOrWhiteSpace(driveLink)
                   && _cache.TryGetValue(driveLink, out var entry)
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