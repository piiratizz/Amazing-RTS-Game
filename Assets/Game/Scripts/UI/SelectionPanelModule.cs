using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.UI
{
    public abstract class SelectionPanelModule : MonoBehaviour
    {
        public abstract void Show(List<ISelectable> targets);
        public abstract void Hide();
    }
}