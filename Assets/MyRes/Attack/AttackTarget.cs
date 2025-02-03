using UnityEngine;

class AttackTarget : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float Health = 100f;
    public float Armor = 0f;

    public int Damage(int expect)
    {
        Health -= expect;
        return expect;
    }
}