using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Extension;
using Infrastructure;
using Infrastructure.Networking;
using Models;
using Navigation;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class BoxingLootController : MonoBehaviour
    {
        #region Const

        private const string LootBoxLayer = "LootBox";

        #endregion

        #region Editor
        
        [SerializeField] private Camera _camera;
        [SerializeField] private PrizeView _prizeViewPrefab;
        [SerializeField] private LootBox[] _boxes;
        //[SerializeField] private Transform[] _boxes;
        [SerializeField] private Transform[] _viewsContainers;
        [SerializeField] private Button[] _boxButtons;
        [SerializeField] private LevelModel _levelModel;

        [Header("UI View")]
        [SerializeField] private ViewCounter _keyCounter;
        [SerializeField] private ViewCounter _energyCounter;
        [SerializeField] private ViewCounter _coinCounter;

        [SerializeField] private Image _image;
        #endregion

        #region Private Variables

        private LayerMask _boxLayer;
        private float _interactionDistance = 1000f;
        private int _stopAfterNButtonsClicked;

        #endregion

        #region Methods

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            _boxLayer = 1 << LayerMask.NameToLayer(LootBoxLayer);

        }

        private async void Start()
        {
            InitButtons().Forget();
            InitLevel();
            // var n = await NetworkManager.GetSpriteAsync(NetworkNavigation.KEY_IMAGE_LINK);
            // _image.sprite = n;
        }

        private async UniTask InitButtons()
        {
            Debug.Log($"[BoxingLootController] InitButtons() ");
            RunFlowAsync().Forget();
        }

        private void InitLevel()
        {
            var shuffledPrizes = _levelModel.Prizes.Shuffled();

            for (int i = 0; i < shuffledPrizes.Count; i++)
            {
                var prize = Instantiate(shuffledPrizes[i].PrizeViewPrefab, _viewsContainers[i]);
                prize.SetData(shuffledPrizes[i]);
            }
        }

        private void Update()
        {
            return;
            if (Input.GetMouseButtonDown(0))
            {
                //ProcessPointer(Input.mousePosition);

                var pos = (Vector2)Input.mousePosition;
                var ray = _camera.ScreenPointToRay(pos);

                if (Physics.Raycast(ray, out var hit, _interactionDistance, _boxLayer, QueryTriggerInteraction.Ignore))
                {
                    Debug.Log($"[BoxingLootController] Update() Raycast Hit");
                    if (hit.collider.TryGetComponent<IInteractable>(out var interact))
                    {
                        interact.Interact();
                    }
                }
            }
        }

        private async UniTaskVoid RunFlowAsync()
        {
            if (_boxButtons == null || _boxButtons.Length == 0)
                return;

            var token = this.GetCancellationTokenOnDestroy();
            var clicked = new bool[_boxButtons.Length];
            _stopAfterNButtonsClicked = clicked.Length;
            
            // Initial state
            SetAllInteractable(true);
            // for (var i = 0; i < _boxButtons.Length; i++)
            // {
            //     if (_boxButtons[i] != null)
            //         _boxButtons[i].interactable = true;
            // }

            var handled = 0;

            while (true)
            {
                if (AllClicked(clicked))
                    break;
                if (_stopAfterNButtonsClicked > 0 && handled >= _stopAfterNButtonsClicked)
                    break;

                // Build tasks ONLY for eligible buttons (not clicked & interactable)
                var waiting = new List<(int index, UniTask task)>();
                for (var i = 0; i < _boxButtons.Length; i++)
                {
                    if (!clicked[i] && _boxButtons[i] != null && _boxButtons[i].interactable)
                    {
                        waiting.Add((i, _boxButtons[i].OnClickAsync(token)));
                    }
                }

                if (waiting.Count == 0)
                    break;

                var taskArray = waiting.Select(w => w.task).ToArray();
                var any = await UniTask.WhenAny(taskArray);
                var winnerIndex = waiting[any].index;

                // Lock all during your async handler to avoid re-entrancy
                SetAllInteractable(false);

                try
                {
                    await OnButtonChosenAsync(winnerIndex, _boxButtons[winnerIndex], token);
                }
                finally
                {
                    // Mark clicked -> keep disabled; re-enable the rest
                    clicked[winnerIndex] = true;
                    if (_boxButtons[winnerIndex] != null)
                        _boxButtons[winnerIndex].interactable = false;

                    for (var i = 0; i < _boxButtons.Length; i++)
                    {
                        if (!clicked[i] && _boxButtons[i] != null)
                            _boxButtons[i].interactable = true;
                    }
                }

                handled++;
            }

            Debug.Log("Done: all clicked or stopped.");
        }

        private async UniTask OnBoxClicked(int index, Button btn, CancellationToken token)
        {
            //TODO: Animate
            //TODO: Set UI
            //TODO: Set Sound
            //TODO: Set Partical system
        }
        
        // Your per-click async work (animation, UI updates, SFX, logic)
        private async UniTask OnButtonChosenAsync(int index, Button btn, CancellationToken token)
        {
            Debug.Log($"Chosen index: {index}, button: {btn?.name}");

            // Example: DOTween punch + small settle
            
            if (_boxes[index] != null)
            {
                await _boxes[index].Interact();
                // var t = _boxes[index].transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.25f, 10, 0.9f);
                // await t.AsyncWaitForCompletion();
            }

            // TODO: your UI updates here (text, sprites, sounds)
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token);
        }

        private void SetAllInteractable(bool value)
        {
            for (var i = 0; i < _boxButtons.Length; i++)
            {
                if (_boxButtons[i] != null)
                    _boxButtons[i].interactable = value;
            }
        }

        private bool AllClicked(bool[] clicks)
        {
            foreach (var isClicked in clicks)
            {
                if (!isClicked)
                    return false;
            }

            return true;
        }

        #endregion
    }
}