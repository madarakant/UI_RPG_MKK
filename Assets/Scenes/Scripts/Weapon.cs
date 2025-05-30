using UnityEngine;

[System.Serializable]
public abstract class Weapon : MonoBehaviour
{
    protected string weaponName;
    protected int baseDamage;
    protected float critChance;

    protected virtual void Awake() 
    {
        // Base awake implementation (can be empty)
    }

    public abstract int CalculateDamage();
    public abstract string GetWeaponInfo();
}