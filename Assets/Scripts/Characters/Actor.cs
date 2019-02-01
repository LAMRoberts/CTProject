using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField]
    private float health = 100.0f;
    [SerializeField]
    private float damageResistance = 0.0f; // 0.0 to 1.0
    [SerializeField]
    private float stamina;
    [SerializeField]
    private bool inSideRoom = false;
    
    #region Getters

    public float GetHealth()
    {
        return health;
    }

    public float GetStamina()
    {
        return stamina;
    }

    public bool GetSideRoom()
    {
        return inSideRoom;
    }

    #endregion

    public void TakeDamage(float damage)
    {
        Debug.Log("Ow, What the fuck is wrong with you? Health: " + health);

        float dmg = damage - (damage * damageResistance);

        if (health > dmg)
        {
            health -= dmg;
        }
        else
        {
            health = 0.0f;
        }
    }

    public void SetSideRoom(bool sr)
    {
        inSideRoom = sr;
    }
}
