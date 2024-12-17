using System;
using UnityEngine;

public static class DamageEvent
{
    public static event Action<float> OnDamage;

    public static void DealDamage(float damage) 
    {
        OnDamage?.Invoke(damage);
    }
}
