using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Services
{
    public class LoadingScreenLocator
    {
        private static ILoadingScreen _service;
        public static async UniTask<ILoadingScreen> Get()
        {
            if (_service != null)
                return _service;
            
            await SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            var service = Object.FindObjectsOfType<MonoBehaviour>().OfType<ILoadingScreen>().FirstOrDefault();
            if (service == null)
            {
                throw new Exception("Can't locate Loading Screen service");
            }
            
            _service = service;
            return _service;
        }
    }
}