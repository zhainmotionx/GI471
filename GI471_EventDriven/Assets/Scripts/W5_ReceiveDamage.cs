using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W5_ReceiveDamage : MonoBehaviour
{
    public W5_StatusSystem statusSystem;
    public float def = 0.0f;

    public void TakeDamage(GameObject damageFrom, float inDamage)
    {
        inDamage -= def;

        if(inDamage <= 0.0f)
        {
            inDamage = 0.0f;
        }

        statusSystem.TakeDamage(damageFrom, inDamage);
    }
}
