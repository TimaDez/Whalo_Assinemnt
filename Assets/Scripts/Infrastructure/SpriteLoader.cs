using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infrastructure.Networking;
using UnityEngine;

namespace Infrastructure
{
    public static class SpriteLoader
    {
        // In-memory cache keyed by the original Drive link
        private static readonly Dictionary<string, (Texture2D tex, Sprite sprite)> _cache = new();

        /// <summary>
        /// 1) If not downloaded, downloads via DriveImageLoader and saves to RAM cache.
        /// 2) Returns a Sprite for the given Drive link.
        /// </summary>
        public static async UniTask<Sprite> GetSpriteAsync(string driveLink, CancellationToken token = default, float ppu = 100f)
        {
            if (string.IsNullOrWhiteSpace(driveLink))
                throw new System.ArgumentException("driveLink is null/empty.", nameof(driveLink));

            // Already cached?
            if (_cache.TryGetValue(driveLink, out var entry) && entry.sprite != null)
                return entry.sprite;

            // Not cached → load Texture2D (this uses disk cache inside your DriveImageLoader)
            var tex = await NetworkManager.GetTextureAsync(driveLink, token);

            // Make a sprite and store both
            var rect = new Rect(0, 0, tex.width, tex.height);
            var sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), ppu);
            _cache[driveLink] = (tex, sprite);
            return sprite;
        }

        /// <summary>
        /// 2) Check if a drive link is already in the in-memory cache.
        /// </summary>
        public static bool IsCached(string driveLink)
        {
            return !string.IsNullOrWhiteSpace(driveLink)
                   && _cache.TryGetValue(driveLink, out var entry)
                   && entry.sprite != null;
        }

        /// <summary>
        /// 3) Release all cached images (destroys Sprites & Textures and clears RAM).
        /// </summary>
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