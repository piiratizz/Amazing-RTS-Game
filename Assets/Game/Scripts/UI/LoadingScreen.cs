using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider progressBar;

        private void Start()
        {
            Hide();
        }

        public void Show()
        {
            loadingScreen.SetActive(true);
        }

        public void UpdateProgress(float progress)
        {
            progressBar.value = progress;
        }
        
        public void Hide()
        {
            loadingScreen.SetActive(false);
        }
    }
}