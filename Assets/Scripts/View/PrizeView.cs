
using Cysharp.Threading.Tasks;
using DataTypes;
using Infrastructure;
using Models;
using Navigation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whalo.Models;

namespace Whalo.UI
{
    public class PrizeView : MonoBehaviour
    {
        #region Editor

        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI  _text;

        #endregion
        
        #region Methods
        
        public async void SetData(PrizeModel model)
        {
            _text.text = model.Amount.ToString();
            _image.sprite = await GetImage(model);
            gameObject.SetActive(false);
        }

        private async UniTask<Sprite> GetImage(PrizeModel model)
        {
            switch (model.Type)
            {
                case PrizeType.None:
                    break;
                case PrizeType.Key:
                    return await SpriteLoader.GetSpriteAsync(NetworkNavigation.KEY_IMAGE_LINK, this.GetCancellationTokenOnDestroy());
                case PrizeType.Gems:
                    return await SpriteLoader.GetSpriteAsync(NetworkNavigation.ENERGY_IMAGE_LINK, this.GetCancellationTokenOnDestroy());
                case PrizeType.Coins:
                    return await SpriteLoader.GetSpriteAsync(NetworkNavigation.COINS_IMAGE_LINK, this.GetCancellationTokenOnDestroy());
            }

            Debug.LogError($"[PrizeView] GetImage() didn't find image for reward type: {model.Type}");
            return null;
        }
        
        #endregion

    }
}