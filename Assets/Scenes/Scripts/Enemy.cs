using TMPro;
using UnityEngine;

public abstract class Enemy : Character
{
    [SerializeField] protected TMP_Text _statsText;
    [SerializeField] protected TMP_Text _battleLogText;
    protected Weapon weapon;
    public object OnDeathEvent { get; set; }

    public void SetTextComponents(TMP_Text statsText, TMP_Text battleLogText)
    {
        _statsText = statsText;
        _battleLogText = battleLogText;
        UpdateStatsDisplay();
    }

    protected virtual void Start()
    {
        weapon = GetComponent<Weapon>();
        InitializeEnemy();
    }

    protected abstract void InitializeEnemy();

    public abstract void Attack(Player player);

    public override string GetCharacterStats()
    {
        string weaponInfo = weapon != null ? weapon.GetWeaponInfo() : "Claws";
        return $"{CharacterName}\nHP: {CurrentHealth}/{MaxHealth}\nWeapon: {weaponInfo}";
    }

    public void UpdateStatsDisplay()
    {
        if (_statsText != null)
        {
            _statsText.text = GetCharacterStats();
        }
    }

    protected void AppendToBattleLog(string message)
    {
        if (_battleLogText != null)
        {
            _battleLogText.text += message;
        }
    }

    public void SetCharacterName(string fallback)
    {
        throw new System.NotImplementedException();
    }
}