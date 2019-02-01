using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public GameObject me;

    public float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != me)
        {
            if (other.gameObject.GetComponent<Actor>())
            {
                other.gameObject.GetComponent<Actor>().TakeDamage(damage);
            }
        }
    }
}
