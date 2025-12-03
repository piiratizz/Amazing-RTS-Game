using System;
using TMPro;
using UnityEngine;

namespace MainMenu
{
    public class LobbyPlayerView : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown playersDropdown;
        [SerializeField] private int playerId;
        
        public int DropdownValue => playersDropdown.value;
        
        public event Action<int, int> OnColorChanged;
        
        public void OnPlayerColorChanged(int index)
        {
            OnColorChanged?.Invoke(playerId, index);
        }
    }
}