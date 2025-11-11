using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ViewCounter : MonoBehaviour
    {
        #region Editor

        [SerializeField] private TextMeshProUGUI _amount;
        [SerializeField] private Image _image;

        #endregion

        #region Methods

        public void Init(Sprite sprite, int amount = 0)
        {
            _image.sprite = sprite;
            _amount.text = amount.ToString();
        }
        
        public void SetAmount(int amount)
        {
            _amount.text = amount.ToString();
        }

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }
        
        #endregion

    }
}