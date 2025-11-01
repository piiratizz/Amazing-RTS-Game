using UnityEngine;
using UnityEngine.VFX;

public class ArrowVFXTrigger : MonoBehaviour
{
    [SerializeField] private VisualEffect effect;
    
    private void Start()
    {
        effect.Stop();
    }

    public void Launch(Vector3 spawnPosition, Vector3 destination, float speed, float lifetime, float heightModifier)
    {
        effect.SetFloat("ArrowLifetime", lifetime);
        effect.SetVector3("ArrowDestination", destination);
        effect.SetVector3("ArrowSpawnPosition", spawnPosition);
        effect.SetFloat("ArrowSpeed", speed);
        effect.SetFloat("ArrowHeightModifier", heightModifier);
        
        effect.Play();
    }
}
