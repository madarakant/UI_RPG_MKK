using UnityEngine;

[System.Serializable]
public class Sword : Weapon
{
    [Header("Sword Properties")]
    [SerializeField] private string swordName = "Steel Sword";
    [SerializeField] private int baseSwordDamage = 15;
    [SerializeField] private float criticalChance = 0.15f;
    [SerializeField] private float parryChance = 0.2f;
    [SerializeField] private ParticleSystem swordGlowEffect;
    
    [Header("Damage Multipliers")]
    [SerializeField] private float critMultiplier = 2f;
    [SerializeField] private float weakSpotMultiplier = 1.5f;

    private bool isParrying = false;

    protected override void Awake()
    {
        base.Awake();
        weaponName = swordName;
        baseDamage = baseSwordDamage;
        critChance = criticalChance;
        
        if (swordGlowEffect != null)
        {
            swordGlowEffect.Stop();
        }
    }

    public override int CalculateDamage()
    {
        bool isCritical = Random.value < critChance;
        bool hitWeakSpot = Random.value < 0.3f; // 30% chance to hit weak spot
        
        float damageMultiplier = 1f;
        
        if (isCritical)
        {
            damageMultiplier *= critMultiplier;
            PlaySwordEffect(Color.yellow); // Gold glow for crits
        }
        else if (hitWeakSpot)
        {
            damageMultiplier *= weakSpotMultiplier;
            PlaySwordEffect(Color.white); // White flash for weak spots
        }

        return Mathf.RoundToInt(baseDamage * damageMultiplier);
    }

    public bool AttemptParry()
    {
        isParrying = Random.value < parryChance;
        if (isParrying && swordGlowEffect != null)
        {
            swordGlowEffect.startColor = Color.blue;
            swordGlowEffect.Play();
        }
        return isParrying;
    }

    private void PlaySwordEffect(Color glowColor)
    {
        if (swordGlowEffect != null)
        {
            swordGlowEffect.startColor = glowColor;
            swordGlowEffect.Play();
        }
    }

    public override string GetWeaponInfo()
    {
        return $"{swordName}\n" +
               $"Damage: {baseDamage}\n" +
               $"Crit: {criticalChance * 100}% ({critMultiplier}x)\n" +
               $"Parry: {parryChance * 100}%";
    }
}