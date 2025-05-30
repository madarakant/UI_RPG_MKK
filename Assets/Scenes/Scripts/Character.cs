using UnityEngine;
using System;

// Base class for all characters (demonstrates Inheritance)
public abstract class Character : MonoBehaviour
{
    // Encapsulation: Using properties with getters/setters
    [SerializeField] protected string characterName;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int baseDamage;
    [SerializeField] protected float defenseModifier = 1f;

    public string CharacterName 
    { 
        get => characterName; 
        protected set => characterName = value; 
    }

    public int MaxHealth 
    { 
        get => maxHealth; 
        protected set => maxHealth = value; 
    }

    public int CurrentHealth 
    { 
        get => currentHealth; 
        protected set => currentHealth = Mathf.Clamp(value, 0, maxHealth); 
    }

    public int BaseDamage 
    { 
        get => baseDamage; 
        protected set => baseDamage = value; 
    }

    public float DefenseModifier 
    { 
        get => defenseModifier; 
        set => defenseModifier = Mathf.Clamp(value, 0.1f, 1f); 
    }

    // Polymorphism: Virtual method that can be overridden
    public virtual int CalculateDamage()
    {
        return BaseDamage;
    }

    // Polymorphism: Overloaded method
    public void TakeDamage(int damage)
    {
        int reducedDamage = Mathf.RoundToInt(damage * DefenseModifier);
        CurrentHealth -= reducedDamage;
    }

    // Overloaded version with critical hit chance
    public void TakeDamage(int damage, float critChance)
    {
        bool isCritical = UnityEngine.Random.value < critChance;
        int finalDamage = isCritical ? damage * 2 : damage;
        int reducedDamage = Mathf.RoundToInt(finalDamage * DefenseModifier);
        CurrentHealth -= reducedDamage;
    }

    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }

    // Abstraction: Abstract method to be implemented by derived classes
    public abstract string GetCharacterStats();
}