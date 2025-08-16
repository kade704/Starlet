using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Health : MonoBehaviour
{
    public float maxHP = 100.0f;
    [ReadOnly] public float hp;

    [HideInInspector] public UnityEvent<HealthEventInfo> onDamaged = new UnityEvent<HealthEventInfo>();
    [HideInInspector] public UnityEvent<HealthEventInfo> onHealed = new UnityEvent<HealthEventInfo>();
    [HideInInspector] public UnityEvent<HealthEventInfo> onDeath = new UnityEvent<HealthEventInfo>();

    private void Awake()
    {
        hp = maxHP;
    }

    public void Reset()
    {
        hp = maxHP;
    }

    public void ApplyHeal(float heal, GameObject causer)
    {
        var info = new HealthEventInfo
        {
            causer = causer,
            amount = heal,
        };

        hp += heal;
        if (hp > maxHP) hp = maxHP;

        onHealed.Invoke(info);
    }

    public void ApplyDamage(float damage, GameObject causer)
    {
        var damageInfo = new HealthEventInfo
        {
            causer = causer,
            amount = damage,
        };

        hp -= damage;
        onDamaged.Invoke(damageInfo);
        
        if (hp > 0) return;

        var deathInfo = new HealthEventInfo
        {
            causer = causer,
            amount = damage,
        };

        hp = 0;
        onDeath.Invoke(deathInfo);
    }
}

public struct HealthEventInfo
{
    public GameObject causer;
    public float amount;
}