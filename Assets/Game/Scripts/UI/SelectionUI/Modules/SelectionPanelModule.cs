using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.UI
{
    public abstract class SelectionPanelModule : MonoBehaviour
    {
        public virtual void Initialize(int ownerId) { }
        public abstract void Show(List<Entity> targets);
        public abstract void Hide();
    }
}