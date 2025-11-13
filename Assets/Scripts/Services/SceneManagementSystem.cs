using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Whalo.Services
{
    //public static class SceneManagementSystem<T> where T : MonoBehaviour
    public static class SceneManagementSystem
    {
        private static ILoadingScreen _service;
        //private static T _service;
        private static readonly HashSet<string> _loadedScenes = new();
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
            _loadedScenes.Add(sceneName);
            return _service;
        }

        public static async UniTask LoadSceneAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName);
        }
    }
}