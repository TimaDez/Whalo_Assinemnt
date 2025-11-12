using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Whalo.Services
{
    public static class SceneManagementSystem
    {
        private static ILoadingScreen _service;
        public static async UniTask<ILoadingScreen> Get(string sceneName)
        {
            if (_service != null)
                return _service;
            
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            var service = Object.FindObjectsOfType<MonoBehaviour>().OfType<ILoadingScreen>().FirstOrDefault();
            if (service == null)
            {
                throw new Exception("Can't locate Loading Screen service");
            }
            
            _service = service;
            return _service;
        }

        public static async UniTask LoadSceneAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName);
        }
    }
}