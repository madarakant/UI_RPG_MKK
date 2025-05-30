using UnityEngine;
using TMPro;

public class Kaito : Enemy
{
    [Header("Sword Settings")]
    [SerializeField] private float doubleStrikeChance = 0.2f;
    [SerializeField] private int swordDamageBonus = 3;

    protected override void InitializeEnemy()
    {
        CharacterName = "Kaito";
        MaxHealth = 70;
        CurrentHealth = MaxHealth;
        BaseDamage = 12 + swordDamageBonus; // Sword bonus damage
        
        // Visual setup
        if (TryGetComponent(out SpriteRenderer renderer))
        {
            renderer.sprite = Resources.Load<Sprite>("Sprites/Characters/kaito");
            renderer.sortingLayerName = "Characters";
            renderer.sortingOrder = 0;
        }
    }

    public override void Attack(Player player)
    {
        // Kaito's special sword attack - chance to strike twice
        bool doubleStrike = Random.value < doubleStrikeChance;
        int totalDamage = 0;
        
        for (int i = 0; i < (doubleStrike ? 2 : 1); i++)
        {
            int damage = CalculateDamage();
            player.TakeDamage(damage);
            totalDamage += damage;
        }

        if (_battleLogText != null)
        {
            string strikeText = doubleStrike ? $" strikes twice with his sword for {totalDamage} total damage!" : 
                $" slashes with his sword for {totalDamage} damage!";
            _battleLogText.text += $"\nKaito{strikeText}";
        }
    }

    public override int CalculateDamage()
    {
        // Sword has a chance for clean hits (more consistent damage)
        bool cleanHit = Random.value < 0.7f;
        return cleanHit ? BaseDamage : Mathf.RoundToInt(BaseDamage * 0.8f);
    }
}