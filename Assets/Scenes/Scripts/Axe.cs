using UnityEngine;

[System.Serializable]
public class Axe : Weapon
{
    [Header("Axe Properties")]
    [SerializeField] private string axeName = "Battle Axe";
    [SerializeField] private int baseAxeDamage = 20;
    [SerializeField] private float criticalChance = 0.1f;
    [SerializeField] private float armorPenetration = 0.4f;
    [SerializeField] private float cleaveChance = 0.25f;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem bloodSplatterEffect;
    [SerializeField] private ParticleSystem woodChipEffect;
    [SerializeField] private float screenShakeIntensity = 0.5f;
    
    [Header("Damage Ranges")]
    [SerializeField] private int minDamageVariance = -3;
    [SerializeField] private int maxDamageVariance = 5;

    protected override void Awake()
    {
        base.Awake();
        weaponName = axeName;
        baseDamage = baseAxeDamage;
        critChance = criticalChance;
    }

    public override int CalculateDamage()
    {
        // Base damage with random variance
        int damage = baseDamage + Random.Range(minDamageVariance, maxDamageVariance + 1);
        
        // Critical hit calculation
        bool isCritical = Random.value < critChance;
        if (isCritical)
        {
            damage *= 2;
            TriggerBloodSplatter();
            CameraShake.ShakeCamera(screenShakeIntensity * 1.5f, 0.3f);
        }
        
        // Cleave effect
        if (Random.value < cleaveChance)
        {
            damage = Mathf.RoundToInt(damage * 1.3f);
            TriggerWoodChipEffect();
        }
        
        return Mathf.Max(1, damage); // Ensure at least 1 damage
    }

    public float GetArmorPenetration()
    {
        return armorPenetration;
    }

    private void TriggerBloodSplatter()
    {
        if (bloodSplatterEffect != null)
        {
            bloodSplatterEffect.Play();
        }
    }

    private void TriggerWoodChipEffect()
    {
        if (woodChipEffect != null)
        {
            woodChipEffect.Play();
        }
    }

    public override string GetWeaponInfo()
    {
        return $"{axeName}\n" +
               $"Damage: {baseDamage} ({minDamageVariance} to +{maxDamageVariance})\n" +
               $"Crit: {criticalChance * 100}% (2x)\n" +
               $"Armor Pen: {armorPenetration * 100}%\n" +
               $"Cleave: {cleaveChance * 100}% (+30%)";
    }
}

public static class CameraShake
{
    public static void ShakeCamera(float intensity, float duration)
    {
        // Implement camera shake using your preferred method:
        // 1. For Cinemachine:
        //    CinemachineImpulseSource.Instance.GenerateImpulse(intensity);
        
        // 2. For simple transform shake:
        //    Camera.main.transform.DOShakePosition(duration, intensity);
        
        Debug.Log($"Camera shake - Intensity: {intensity}, Duration: {duration}");
    }
}