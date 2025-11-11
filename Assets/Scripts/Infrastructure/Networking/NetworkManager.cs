using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Infrastructure.Networking
{
    public static class NetworkManager
    {
        #region Private members
        
        private static readonly string CacheDir = Path.Combine(Application.persistentDataPath, "images_cache");

        private static readonly Regex FileIdRegex = new Regex(
            @"(?:/d/([a-zA-Z0-9_-]{10,}))|(?:[?&]id=([a-zA-Z0-9_-]{10,}))",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );
        #endregion

        #region Methods
        /// <summary>
        /// Returns a Texture2D from a Google Drive link (public file).
        /// Caches to disk so next calls are instant. Throws on failure.
        /// </summary>
        public static async UniTask<Texture2D> GetTextureAsync(string link, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(link))
                throw new ArgumentException("Drive link is null or empty.", nameof(link));

            EnsureCacheDir();

            var fileId = ExtractFileId(link);
            if (string.IsNullOrEmpty(fileId))
                throw new Exception("Could not extract Google Drive file ID from the provided link.");

            var cachePath = GetCachePath(fileId);

            // 1) Disk cache
            if (File.Exists(cachePath))
            {
                var bytes = await File.ReadAllBytesAsync(cachePath, token);
                return BytesToTexture(bytes);
            }

            // 2) Download
            var url = $"https://drive.google.com/uc?export=download&id={fileId}";
            var bytesDownloaded = await DownloadBytesAsync(url, token);

            // 3) Save cache (best-effort)
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
                await File.WriteAllBytesAsync(cachePath, bytesDownloaded, token);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[DriveImageLoader] Failed to write cache: {e.Message}");
            }

            // 4) Decode
            return BytesToTexture(bytesDownloaded);
        }

        /// <summary>
        /// Convenience helper to get a Sprite (pivot center, pixelsPerUnit = 100).
        /// </summary>
        public static async UniTask<Sprite> GetSpriteAsync(string link, CancellationToken token = default, float pixelsPerUnit = 100f)
        {
            var tex = await GetTextureAsync(link, token);
            var rect = new Rect(0, 0, tex.width, tex.height);
            var pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(tex, rect, pivot, pixelsPerUnit);
        }

        /// <summary>Deletes the cached file for this link (if exists).</summary>
        public static bool InvalidateCache(string link)
        {
            var id = ExtractFileId(link);
            if (string.IsNullOrEmpty(id))
                return false;

            var path = GetCachePath(id);
            if (!File.Exists(path))
                return false;
            
            File.Delete(path);
            return true;
        }

        private static string ExtractFileId(string link)
        {
            var m = FileIdRegex.Match(link);
            if (!m.Success) return null;

            // group1 = /d/{id}, group2 = ?id={id}
            return !string.IsNullOrEmpty(m.Groups[1].Value)
                ? m.Groups[1].Value
                : m.Groups[2].Value;
        }

        private static void EnsureCacheDir()
        {
            if (!Directory.Exists(CacheDir))
                Directory.CreateDirectory(CacheDir);
        }

        private static string GetCachePath(string fileId)
        {
            // store original bytes as .bin (robust for png/jpg/webp)
            var safe = Sanitize(fileId);
            return Path.Combine(CacheDir, $"{safe}.bin");
        }

        private static string Sanitize(string s)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                s = s.Replace(c, '_');
            return s;
        }

        private static Texture2D BytesToTexture(byte[] data)
        {
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!tex.LoadImage(data, markNonReadable: false))
                throw new Exception("Failed to decode image bytes.");

            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            return tex;
        }

        private static async UniTask<byte[]> DownloadBytesAsync(string url, CancellationToken token)
        {
            using var req = UnityWebRequest.Get(url);
            req.timeout = 15;

            var op = req.SendWebRequest();
            
            while (!op.isDone)
            {
                token.ThrowIfCancellationRequested();
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

#if UNITY_2020_2_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception(req.error);
#else
        if (req.isNetworkError || req.isHttpError)
            throw new Exception(req.error);
#endif

            return req.downloadHandler.data;
        }
        
        #endregion
    }
}
