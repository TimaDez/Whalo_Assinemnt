using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Extension;
using Models;
using Navigation;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Whalo.Services;
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
        [SerializeField] private LevelModel _levelModel;
        
        #endregion

        #region Private Variables

        private int _clickedCount = 0;
        private List<PrizeModel> _shuffledPrizes;
        private PlayerModelSingleton _modelSingleton;
        
        #endregion

        #region Methods

        private void Awake()
        {
            _modelSingleton =  PlayerModelSingleton.Instance;
            _lootBoxesView.Initialize(_levelModel);
        }

        private async void Start()
        {
            InitButtons();
            InitLevel();
            _modelSingleton.Initialize(0, 0, _levelModel.KeyStarterAmount);
            await _lootBoxesView.InitView();
        }

        private void InitButtons()
        {
            SubscribeButtons();
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
            _modelSingleton.WithdrawKeys(1);
            
            await OnBoxClickAnim(index, this.GetCancellationTokenOnDestroy());

            if (_clickedCount == _boxButtons.Length || _modelSingleton.KeysBalance <= 0)
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
            await SceneManagementSystem.LoadSceneAsync(ScenesNavigation.SUMMERY_SCREEN_NAME);
        }

        private async UniTask OpenBoxFlow(int i)
        {
            var tr = _viewsContainers[i].GetChild(0);
            tr.localScale = Vector3.zero;
            tr.gameObject.SetActive(true);

            await _boxes[i].Interact();
            await tr.DOScale(Vector3.one, 0.2f).AsyncWaitForCompletion();
        }
        
        //create rewardanimation syxsvens Execute
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
            _modelSingleton.AddPrize(prize.Type, prize.Amount);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token);
        }
        
        
        private void SubscribeButtons()
        {
            var count = _boxButtons.Length;
            for (var i = 0; i < count; i++)
            {
                var index = i;
                _boxButtons[i].onClick.AddListener(() => OnButtonClicked(index).Forget());
            }
        }
        
        private void UnsubscribeButtons()
        {
            var count = _boxButtons.Length;
            for (var i = 0; i < count; i++)
            {
                var index = i;
                _boxButtons[i].onClick.RemoveListener(() => OnButtonClicked(index).Forget());
            }
        }

        private void OnDisable()
        {
            UnsubscribeButtons();
            _modelSingleton = null;
        }

        #endregion
    }
}