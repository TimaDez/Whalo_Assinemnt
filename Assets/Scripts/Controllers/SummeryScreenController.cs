using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DataTypes;
using Infrastructure;
using Models;
using Navigation;
using UI;
using UnityEngine;
using Whalo.Services;

namespace Whalo.Controllers
{
    public class SummeryScreenController : MonoBehaviour
    {
        #region Editor

        [Header("Counters")]
        [SerializeField] private ViewCounter[] _counters;
        [SerializeField] private ParticleSystem[] _particle;
        
        #endregion

        #region Private Fields

        private PlayerModelSingleton _playerModel;
        
        #endregion
        #region Methods

        private void Awake()
        {
            Models.PlayerModelSingleton.EnsureInstance();
            _playerModel = PlayerModelSingleton.Instance;
        }

        private void Start()
        {
            StartAnim().Forget();
        }

        private async UniTask StartAnim()
        {
            await SetImages();
            await AnimateCounters();
            await PlayParticleAsync(_particle);
            await UniTask.WaitForSeconds(2f, cancellationToken: this.GetCancellationTokenOnDestroy());
            await SceneManagementSystem.LoadSceneAsync(ScenesNavigation.MENU_SCENE_NAME);
        }

        private async UniTask AnimateCounters()
        {
            var tasks = new List<UniTask>();
            foreach (PrizeType prize in Enum.GetValues(typeof(PrizeType)))
            {
                if (prize == PrizeType.None)
                    continue;
                
                var counter = _counters.FirstOrDefault(c => c.Type == prize);
                if (counter == null)
                {
                    Debug.LogError($"[SummeryScreenController] StartAnim() No ViewCounter for Type {prize}");
                    return;
                }
                tasks.Add(DoBalanceAnimation(prize, counter));
            }
            
            await UniTask.WhenAll(tasks);
        }
        
        private async UniTask SetImages()
        {
            var spriteMap = new Dictionary<PrizeType, Sprite>
            {
                { PrizeType.Key,    await SpriteLoader.GetSpriteAsync(NetworkNavigation.KEY_IMAGE_LINK) },
                { PrizeType.Gems, await SpriteLoader.GetSpriteAsync(NetworkNavigation.ENERGY_IMAGE_LINK) },
                { PrizeType.Coins,  await SpriteLoader.GetSpriteAsync(NetworkNavigation.COINS_IMAGE_LINK) },
            };
            
            foreach (var counter in _counters)
            {
                if (spriteMap.TryGetValue(counter.Type, out var sprite))
                    counter.Init(sprite);
            }
        }

        private async UniTask DoBalanceAnimation(PrizeType prizeType, ViewCounter counter)
        {
            var amount = _playerModel.GetBalance(prizeType);
            var numOfRewards = amount;
            var amountPerReward = 1;
            var remainder = 0;
            if (numOfRewards > 10)
            {
                numOfRewards = 10;
                amountPerReward = amount / 10;
                remainder = amount % 10;
            }
            
            var balance = 0;
            for (var i = 0; i < numOfRewards; i++)
            {
                var nextAmount = i == numOfRewards - 1 ? amountPerReward + remainder : amountPerReward;

                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: this.GetCancellationTokenOnDestroy());
                balance += nextAmount;
                counter.SetAmount(balance);
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: this.GetCancellationTokenOnDestroy());
        }
        
        private async UniTask PlayParticleAsync(ParticleSystem[] systems, float delayBetweenStarts = 0.1f, bool delayFirst = false)
        {
            if (systems == null || systems.Length == 0)
                return;

            var tasks = new List<UniTask>(systems.Length);

            for (int i = 0; i < systems.Length; i++)
            {
                var p = systems[i];
                if (p == null)
                    continue;

                var startDelay = TimeSpan.FromSeconds((delayFirst ? (i + 1) : i) * delayBetweenStarts);
                tasks.Add(LaunchParticle(p, startDelay));
            }

            await UniTask.WhenAll(tasks);
        }
        
        private async UniTask LaunchParticle(ParticleSystem ps, TimeSpan delay)
        {
            var token = ps.gameObject.GetCancellationTokenOnDestroy();

            if (delay > TimeSpan.Zero)
                await UniTask.Delay(delay, cancellationToken: token);

            ps.Play(true);

            await UniTask.WaitUntil(() => !ps.IsAlive(true), cancellationToken: token);
        }
        #endregion
    }
}