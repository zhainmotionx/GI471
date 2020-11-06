using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W5_StatusSystem : MonoBehaviour
{
    public float hp;

    public delegate void DelegateHandleTakeDamage(GameObject damageFrom, float inDamge);
    public delegate void DelegateHandleDead();

    public event DelegateHandleTakeDamage OnTakeDamage;
    public event DelegateHandleDead OnDead;

    public void TakeDamage(GameObject damageFrom, float inDamage)
    {
        if(hp > 0)
        {
            hp -= inDamage;

            if(OnTakeDamage != null)
            {
                OnTakeDamage(damageFrom, inDamage);
            }

            if(hp <= 0)
            {
                if(OnDead != null)
                {
                    OnDead();
                }
            }
        }
    }

    public bool IsAlive()
    {
        return hp > 0;
    }
}
