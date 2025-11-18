using System.Collections.Generic;
using UnityEngine;

namespace MainMenu
{
    public class WindowsManager : MainMenuUIModuleBase
    {
        [SerializeField] private List<WindowBase> windows;

        private Dictionary<WindowType, WindowBase> _windowMap = new Dictionary<WindowType, WindowBase>();
        
        private WindowBase _currentWindow;
        
        public override void Initialize(MainMenuHUD hud)
        {
            foreach (var window in windows)
            {
                _windowMap.Add(window.WindowType, window);
                window.Initialize(hud);
                window.Close();
            }
            
            OpenWindow(WindowType.Main);
        }

        public void OpenWindow(WindowType type)
        {
            _currentWindow?.Close();
            _currentWindow = _windowMap[type];
            _currentWindow.Open();
        }
    }
}