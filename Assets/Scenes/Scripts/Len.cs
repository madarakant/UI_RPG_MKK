using UnityEngine;
using TMPro;

public class Len : Enemy
{
    [Header("Fencing Sword Settings")]
    [SerializeField] private float parryChance = 0.25f;
    [SerializeField] private float riposteChance = 0.4f;
    [SerializeField] private int fencingPrecisionBonus = 2;
    
    private bool isParrying = false;

    protected override void InitializeEnemy()
    {
        CharacterName = "Len";
        MaxHealth = 65;
        CurrentHealth = MaxHealth;
        BaseDamage = 14 + fencingPrecisionBonus; // Fencing precision bonus
        
        // Visual setup
        if (TryGetComponent(out SpriteRenderer renderer))
        {
            renderer.sprite = Resources.Load<Sprite>("Sprites/Characters/len");
            renderer.sortingLayerName = "Characters";
            renderer.sortingOrder = 0;
        }
    }

    public override void Attack(Player player)
    {
        // Len's fencing-style combat - chance to parry or riposte
        if (Random.value < parryChance && !isParrying)
        {
            isParrying = true;
            if (_battleLogText != null)
            {
                _battleLogText.text += "\nLen assumes a defensive stance, ready to parry!";
            }
            return;
        }

        if (isParrying && Random.value < riposteChance)
        {
            // Successful riposte
            int damage = CalculateDamage() * 2; // Double damage for riposte
            player.TakeDamage(damage);
            
            if (_battleLogText != null)
            {
                _battleLogText.text += $"\nLen executes a perfect riposte with his fencing sword for {damage} damage!";
            }
        }
        else
        {
            // Standard fencing attack
            int damage = CalculateDamage();
            player.TakeDamage(damage);
            
            if (_battleLogText != null)
            {
                string attackVerb = Random.value < 0.5f ? "lunges" : "feints"; // Fencing-style attacks
                _battleLogText.text += $"\nLen {attackVerb} with his fencing sword for {damage} damage!";
            }
        }
        
        isParrying = false;
    }

    public override int CalculateDamage()
    {
        // Fencing sword has more precise but slightly weaker attacks
        bool preciseStrike = Random.value < 0.8f; // 80% chance for precise strike
        return preciseStrike ? BaseDamage : Mathf.RoundToInt(BaseDamage * 0.7f);
    }

    public override string GetCharacterStats()
    {
        string weaponInfo = weapon != null ? weapon.GetWeaponInfo() : "Fencing Sword";
        return $"{CharacterName}\nHP: {CurrentHealth}/{MaxHealth}\nWeapon: {weaponInfo}\nStance: {(isParrying ? "Parrying" : "Attacking")}";
    }
}