using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public GameObject me;

    public float damage;

    private float currentDamage;

    private void Start()
    {
        currentDamage = damage;
    }

    public void UpdateDamage(float dmg)
    {
        currentDamage = dmg;
    }

    public float GetDamage()
    {
        return damage;
    }

    public void ResetDamage()
    {
        currentDamage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != me)
        {
            if (other.gameObject.GetComponent<Actor>())
            {
                other.gameObject.GetComponent<Actor>().TakeDamage(currentDamage);
            }
        }
    }
}
