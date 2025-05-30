using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player : Character
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _statsText;
    [SerializeField] private TMP_Text _battleLogText;
    
    [Header("Visual Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Image playerImage;

    [Header("Shield Settings")]
    [SerializeField] private float shieldBreakChance = 0.3f;
    [SerializeField] private int shieldDefenseBonus = 50;
    
    public bool ShieldActive { get; private set; }
    public UnityEvent OnDeathEvent { get; private set; } = new UnityEvent();

    private void Start()
    {
        InitializePlayer();
    }

    public void SetTextComponents(TMP_Text statsText, TMP_Text battleLogText)
    {
        _statsText = statsText;
        _battleLogText = battleLogText;
        UpdateStatsDisplay();
    }

    private void InitializePlayer()
    {
        CharacterName = "Hero";
        MaxHealth = 200;
        CurrentHealth = MaxHealth;
        BaseDamage = 20;
        DefenseModifier = 1f;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Characters/hero");
            spriteRenderer.sortingLayerName = "Characters";
            spriteRenderer.sortingOrder = 1;
        }
        UpdateStatsDisplay();
    }

    public override string GetCharacterStats()
    {
        return $"{CharacterName}\nHP: {CurrentHealth}/{MaxHealth}\nDamage: {BaseDamage}\nShield: {(ShieldActive ? "ON" : "OFF")}";
    }

    public void UpdateStatsDisplay()
    {
        if (_statsText != null)
        {
            _statsText.text = GetCharacterStats();
        }
    }

    public void ResetPlayer()
    {
        CurrentHealth = MaxHealth;
        ShieldActive = false;
        DefenseModifier = 1f;
        UpdateStatsDisplay();
        
        if (_battleLogText != null)
        {
            _battleLogText.text = "Game restarted!\n";
        }
        SetVisible(true);
    }

    public void SetVisible(bool visible)
    {
        // Handle SpriteRenderer case
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = visible;
        }
        
        // Handle UI Image case
        if (playerImage != null)
        {
            playerImage.enabled = visible;
        }
    
        // Optional: If you have any child images
        var childImages = GetComponentsInChildren<Image>(true);
        foreach (var image in childImages)
        {
            image.enabled = visible;
        }
    }

    public void ToggleShield()
    {
        ShieldActive = !ShieldActive;
        DefenseModifier = ShieldActive ? (1f - shieldDefenseBonus/100f) : 1f;
        
        if (_battleLogText != null)
        {
            _battleLogText.text += ShieldActive ? "\nShield activated!" : "\nShield deactivated!";
        }
        UpdateStatsDisplay();
    }

    public void Attack(Enemy enemy)
    {
        int damage = CalculateDamage();
        enemy.TakeDamage(damage);
        
        if (_battleLogText != null)
        {
            _battleLogText.text += $"\nYou attack for {damage} damage!";
        }
        
        if (ShieldActive && Random.value < shieldBreakChance)
        {
            ShieldActive = false;
            DefenseModifier = 1f;
            if (_battleLogText != null)
            {
                _battleLogText.text += "\nYour shield broke!";
            }
            UpdateStatsDisplay();
        }
    }

    public override int CalculateDamage()
    {
        return BaseDamage;
    }

    // Using new instead of override since base method isn't virtual
    public new void TakeDamage(int damage)
    {
        // Apply defense modifier
        int finalDamage = Mathf.RoundToInt(damage * DefenseModifier);
        CurrentHealth = Mathf.Max(0, CurrentHealth - finalDamage);
        
        if (_battleLogText != null)
        {
            _battleLogText.text += $"\nYou take {finalDamage} damage!";
        }
        
        if (IsDead())
        {
            OnDeathEvent.Invoke();
        }
        
        UpdateStatsDisplay();
    }

    // Using new instead of override since base method isn't virtual
    public new bool IsDead()
    {
        return CurrentHealth <= 0;
    }
}