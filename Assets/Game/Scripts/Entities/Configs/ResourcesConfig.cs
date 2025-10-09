using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesConfig",  menuName = "Configs/ResourcesConfig")]
public class ResourcesConfig : EntityConfig
{
    public ResourceType ResourceType;
    public int Amount;
    public float SizeX;
    public float SizeZ;
}