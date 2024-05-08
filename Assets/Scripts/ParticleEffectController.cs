using UnityEngine;

public class ParticleEffectController : MonoBehaviour
{
    [SerializeField] private GameObject _particleEffectPrefab;
    
    public void CreateParticleEffect(Vector3 position)
    {
        var particleEffect = Instantiate(_particleEffectPrefab, position, Quaternion.identity);
        particleEffect.SetActive(true);
        Destroy(particleEffect, 2f);
    }
}