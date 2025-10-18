using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData",  menuName = "Configs/ResourceData")]
public class ResourceData : ScriptableObject
{
    public string ResourceName;
    public Sprite UiDisplayIcon;
    public ResourceType ResourceType;
}