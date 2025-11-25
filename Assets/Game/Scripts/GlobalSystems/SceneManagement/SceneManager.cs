using Cysharp.Threading.Tasks;
using Game.Scripts.UI;
using UnityEngine;
using Zenject;

namespace Game.Scripts.GlobalSystems.SceneManagement
{
    public class SceneManager
    {
        [Inject] private LoadingScreen _loadingScreenInstance;

        private AsyncOperation _loadingOperation;

        public async UniTaskVoid SwitchScene(string sceneName)
        {
            _loadingScreenInstance.Show();

            _loadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            _loadingOperation.allowSceneActivation = false;
            while (_loadingOperation.progress < 0.9f)
            {
                Debug.Log(_loadingOperation.progress);
                _loadingScreenInstance.UpdateProgress(_loadingOperation.progress);
                await UniTask.Yield();
            }

            _loadingScreenInstance.UpdateProgress(1f);

            _loadingOperation.allowSceneActivation = true;

            await _loadingOperation;

            _loadingScreenInstance.Hide();
        }
    }
}