using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Extension;
using Models;   
using UI;
using UnityEngine;
using UnityEngine.UI;
using Whalo.UI;

namespace Whalo.Controllers
{
    public class BoxingLootController : MonoBehaviour
    {
        #region Editor

        [SerializeField] private LootBoxesView _lootBoxesView;
        [SerializeField] private LootBox[] _boxes;
        [SerializeField] private Transform[] _viewsContainers;
        [SerializeField] private Button[] _boxButtons;
        
        [Header("Models")]
        [SerializeField] private PlayerModel _playerModel;
        [SerializeField] private LevelModel _levelModel;
        
        #endregion

        #region Private Variables

        private int _clickedCount = 0;
        private List<PrizeModel> _shuffledPrizes;

        #endregion

        #region Methods

        private void Awake()
        {
            _lootBoxesView.Initialize(_levelModel, _playerModel);
        }

        private async void Start()
        {
            InitButtons();
            InitLevel();
            _playerModel.Initialize(0, 0 ,_levelModel.KeyStarterAmount);
            await _lootBoxesView.InitView();
        }

        private void InitButtons()
        {
            AddListenerToButton(true);
        }

        private void AddListenerToButton(bool add)
        {
            var count = _boxButtons.Length;
            for (var i = 0; i < count; i++)
            {
                var index = i;
                if(add)
                    _boxButtons[i].onClick.AddListener(() => OnButtonClicked(index).Forget());
                else
                    _boxButtons[i].onClick.RemoveListener(() => OnButtonClicked(index).Forget());
            }
        }
        
        private void InitLevel()
        {
            _shuffledPrizes = _levelModel.Prizes.Shuffled();

            for (int i = 0; i < _shuffledPrizes.Count; i++)
            {
                var prize = Instantiate(_shuffledPrizes[i].PrizeViewPrefab, _viewsContainers[i]);
                prize.SetData(_shuffledPrizes[i]);
            }
        }

        private async UniTaskVoid OnButtonClicked(int index)
        {
            if (_boxes[index].IsOpen)
                return;

            _clickedCount++;

            Debug.Log($"Button clicked: {index}");

            _boxButtons[index].interactable = false;
            _playerModel.WithdrawKeys(1);
            
            // var prize = _shuffledPrizes[index];
            // _playerModel.AddPrize(prize.Type, prize.Amount);
            
            await OnBoxClickAnim(index, this.GetCancellationTokenOnDestroy());

            if (_clickedCount == _boxButtons.Length || _playerModel.KeysBalance <= 0)
            {
                Debug.Log("ALL BUTTONS WERE CLICKED!");
                await OnLevelEnd();
                Debug.Log($"[BoxingLootController] OnButtonClicked() All boxes opened");
            }
        }

        private async UniTask OnLevelEnd()
        {
            var tasks = new List<UniTask>();

            for (var i = 0; i < _boxes.Length; i++)
            {
                if (_boxes[i].IsOpen)
                    continue;

                tasks.Add(OpenBoxFlow(i));
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask OpenBoxFlow(int i)
        {
            var tr = _viewsContainers[i].GetChild(0);
            tr.localScale = Vector3.zero;
            tr.gameObject.SetActive(true);

            await _boxes[i].Interact();
            await tr.DOScale(Vector3.one, 0.2f).AsyncWaitForCompletion();
        }
        
        private async UniTask OnBoxClickAnim(int index, CancellationToken token)
        {
            //TODO: Animate
            //TODO: Set UI
            //TODO: Set Sound
            //TODO: Set Partical system
            //TODO: check if no more keys left
            
            if (_boxes[index] != null)
            {
                await _boxes[index].Interact();
                _viewsContainers[index].GetChild(0).gameObject.SetActive(true);
            }

            var prize = _shuffledPrizes[index];
            await _lootBoxesView.FlyFrom(prize.Type, _viewsContainers[index], prize.Amount);
            _playerModel.AddPrize(prize.Type, prize.Amount);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token);
        }
        
        private void OnDisable()
        {
            AddListenerToButton(false);
        }

        #endregion
    }
}