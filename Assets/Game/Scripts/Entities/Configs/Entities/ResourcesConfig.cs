using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesConfig",  menuName = "Configs/ResourcesConfig")]
public class ResourcesConfig : EntityConfig
{
    public ResourceData Resource;
    public int Amount;
    public float SizeX;
    public float SizeZ;
}