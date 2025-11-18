using UnityEngine;

namespace MainMenu
{
    public abstract class WindowBase : MonoBehaviour
    {
        [SerializeField] private WindowType windowType;
        
        public WindowType WindowType => windowType;
        
        public virtual void Initialize(MainMenuHUD hud) { }
        
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
