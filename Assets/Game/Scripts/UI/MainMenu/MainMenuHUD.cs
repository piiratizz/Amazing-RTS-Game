using System.Collections.Generic;
using UnityEngine;

namespace MainMenu
{
    public class MainMenuHUD : MonoBehaviour
    {
        [SerializeField] private List<MainMenuUIModuleBase> uiModules;

        public void Initialize()
        {
            foreach (var module in uiModules)
            {
                module.Initialize(this);
            }
        }

        public TModule GetModule<TModule>() where TModule : MainMenuUIModuleBase
        {
            foreach (var comp in uiModules)
                if (comp is TModule tComp) 
                    return tComp;
            
            return null;
        }
    }
}