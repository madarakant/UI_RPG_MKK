using UnityEngine;

[System.Serializable]
public class FencingSaber : Weapon
{
    [Header("Fencing Properties")]
    [SerializeField] private string saberName = "Duelling Saber";
    [SerializeField] private int baseSaberDamage = 12;
    [SerializeField] private float criticalChance = 0.25f; // Higher precision
    [SerializeField] private float parryWindow = 0.3f; // Seconds
    [SerializeField] private float riposteDamageMultiplier = 2.5f;
    
    [Header("Fencing Effects")]
    [SerializeField] private ParticleSystem lungeEffect;
    [SerializeField] private ParticleSystem parrySparkEffect;
    [SerializeField] private AudioClip[] fencingSounds;
    
    [Header("Precision Modifiers")]
    [SerializeField] private float weakSpotChance = 0.4f;
    [SerializeField] private float precisionBonus = 0.8f; // 80% consistent damage

    private float parryTimer = 0f;
    private bool isParrying = false;

    protected override void Awake()
    {
        base.Awake();
        weaponName = saberName;
        baseDamage = baseSaberDamage;
        critChance = criticalChance;
    }

    private void Update()
    {
        if (isParrying)
        {
            parryTimer -= Time.deltaTime;
            if (parryTimer <= 0f)
            {
                isParrying = false;
            }
        }
    }

    public override int CalculateDamage()
    {
        bool isCritical = Random.value < critChance;
        bool hitWeakSpot = Random.value < weakSpotChance;
        bool preciseStrike = Random.value < precisionBonus;

        float damageMultiplier = 1f;
        
        if (isCritical)
        {
            damageMultiplier *= 3f; // Devastating criticals
            PlayLungeEffect(Color.magenta);
        }
        else if (hitWeakSpot)
        {
            damageMultiplier *= 1.7f;
            PlayLungeEffect(Color.cyan);
        }
        else if (preciseStrike)
        {
            damageMultiplier *= 1f; // Base damage
        }
        else
        {
            damageMultiplier *= 0.6f; // Poor execution
        }

        return Mathf.RoundToInt(baseDamage * damageMultiplier);
    }

    public bool AttemptParry()
    {
        if (!isParrying)
        {
            isParrying = true;
            parryTimer = parryWindow;
            PlayParrySpark();
            return true;
        }
        return false;
    }

    public int ExecuteRiposte()
    {
        int damage = Mathf.RoundToInt(CalculateDamage() * riposteDamageMultiplier);
        PlayLungeEffect(Color.red);
        return damage;
    }

    private void PlayLungeEffect(Color tint)
    {
        if (lungeEffect != null)
        {
            lungeEffect.startColor = tint;
            lungeEffect.Play();
            PlayFencingSound(0); // Lunge sound
        }
    }

    private void PlayParrySpark()
    {
        if (parrySparkEffect != null)
        {
            parrySparkEffect.Play();
            PlayFencingSound(1); // Metal clash sound
        }
    }

    private void PlayFencingSound(int index)
    {
        if (fencingSounds != null && fencingSounds.Length > index)
        {
            AudioSource.PlayClipAtPoint(fencingSounds[index], transform.position);
        }
    }

    public override string GetWeaponInfo()
    {
        return $"{saberName}\n" +
               $"Precision: {precisionBonus * 100}%\n" +
               $"Crit: {criticalChance * 100}% (3x)\n" +
               $"Parry Window: {parryWindow}s\n" +
               $"Riposte: {riposteDamageMultiplier}x";
    }
}