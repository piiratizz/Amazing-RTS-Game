using MainMenu;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField] private MainMenuHUD mainMenuHUD;
        
        public override void InstallBindings()
        {
            Container.Bind<MainMenuHUD>().AsSingle();
            mainMenuHUD.Initialize();
        }
    }
}