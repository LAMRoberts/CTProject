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

    protected bool dead = false;

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

    public void SetHealth(float value)
    {
        health = value;
    }

    public void TakeDamage(float damage)
    {
        float dmg = damage - (damage * damageResistance);
               
        if (health > dmg)
        {
            health -= dmg;

            Debug.Log("Ouch. Health: " + health + " DMG: " + dmg);
        }
        else
        {
            health = 0.0f;

            Debug.Log("Ouch. Health: 0 DMG: " + dmg);

            Murder();
        }
    }

    protected virtual void Murder()
    {
        dead = true;
    }

    public void Revive()
    {
        health = 100;

        dead = false;
    }

    public void SetSideRoom(bool sr)
    {
        inSideRoom = sr;
    }
}
