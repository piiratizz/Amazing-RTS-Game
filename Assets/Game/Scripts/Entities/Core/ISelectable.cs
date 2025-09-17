using UnityEngine;

public interface ISelectable
{
    int OwnerId { get; }
    string DisplayName { get; }
    Sprite Icon { get; }
    bool IsAvailableToSelect { get; set; }
    void OnSelect(Player selecter);
    void OnDeselect();
    GameObject SelectedObject();
}