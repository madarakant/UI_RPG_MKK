using UnityEngine;
using TMPro;

public class Rin : Enemy
{
    [Header("Axe Combat Settings")]
    [SerializeField] private float cleaveChance = 0.35f;
    [SerializeField] private float defensePenetration = 0.4f; // 40% armor ignore
    [SerializeField] private int axeDamageBonus = 5;
    [SerializeField] private float stumbleChance = 0.15f;

    protected override void InitializeEnemy()
    {
        CharacterName = "Rin";
        MaxHealth = 60;
        CurrentHealth = MaxHealth;
        BaseDamage = 15 + axeDamageBonus; // Axe damage bonus
        
        // Visual setup - fierce axe-wielder appearance
        if (TryGetComponent(out SpriteRenderer renderer))
        {
            renderer.sprite = Resources.Load<Sprite>("Sprites/Characters/rin");
            renderer.sortingLayerName = "Characters";
            renderer.sortingOrder = 0;
        }
    }

    public override void Attack(Player player)
    {
        bool cleavingStrike = Random.value < cleaveChance;
        int damage = CalculateDamage();

        // Axe special: Chance to ignore defense
        if (Random.value < defensePenetration)
        {
            float originalDefense = player.DefenseModifier;
            player.DefenseModifier = 1f; // Bypass defense
            player.TakeDamage(damage);
            player.DefenseModifier = originalDefense;
            
            if (_battleLogText != null)
            {
                _battleLogText.text += $"\nRin's axe SMASHES through defenses for {damage} damage!";
            }
        }
        else
        {
            player.TakeDamage(damage);
            if (_battleLogText != null)
            {
                string attackText = cleavingStrike ? 
                    $" delivers a CLEAVING axe blow for {damage} damage!" :
                    $" swings her axe wildly for {damage} damage!";
                _battleLogText.text += $"\nRin{attackText}";
            }
        }

        // Axe drawback: Chance to stumble after attack
        if (Random.value < stumbleChance)
        {
            BaseDamage = Mathf.RoundToInt(BaseDamage * 0.8f); // Temporary damage reduction
            if (_battleLogText != null)
            {
                _battleLogText.text += "\nRin stumbles from the weight of her axe!";
            }
        }
    }

    public override int CalculateDamage()
    {
        // Axe hits harder but less consistently
        bool solidHit = Random.value < 0.6f;
        int baseResult = solidHit ? BaseDamage : Mathf.RoundToInt(BaseDamage * 0.7f);

        // Chance for bonus cleave damage
        if (Random.value < cleaveChance)
        {
            return baseResult + Mathf.RoundToInt(baseResult * 0.3f);
        }
        return baseResult;
    }

    public override string GetCharacterStats()
    {
        string weaponInfo = weapon != null ? weapon.GetWeaponInfo() : "Heavy Axe";
        string status = BaseDamage < (15 + axeDamageBonus) ? "Stumbling" : "Ready";
        return $"{CharacterName}\nHP: {CurrentHealth}/{MaxHealth}\nWeapon: {weaponInfo}\nStatus: {status}";
    }
}