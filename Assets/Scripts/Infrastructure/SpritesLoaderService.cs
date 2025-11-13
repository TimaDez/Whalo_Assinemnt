using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Infrastructure
{
    public static class SpritesLoaderService
    {
        #region Methods

        public static async UniTask LoadSprites(List<string> urlsToLoad, CancellationToken token = default)
        {
            foreach (var url in urlsToLoad)
            {
                if(!SpriteLoader.IsCached(url))
                    await SpriteLoader.LoadSpriteAsync(url, token);
            }
        }

        #endregion
    }
}