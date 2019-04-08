using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public GameObject me;

    public bool ready = false;

    public const float damage = 25.0f;

    private float currentDamage;

    private List<Actor> attacked;

    private void Start()
    {
        attacked = new List<Actor>();

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
        // if im attacking someone else
        if (ready && other.gameObject != me)
        {
            //Debug.Log("Not me");

            // check they are an actor
            if (other.gameObject.GetComponent<Actor>())
            {
                //Debug.Log("Actor");

                Actor a = other.gameObject.GetComponent<Actor>();

                // if they havent been attacked yet
                if (!attacked.Contains(other.gameObject.GetComponent<Actor>()))
                {
                    //Debug.Log("Attacking");

                    // add them to attacked
                    attacked.Add(other.gameObject.GetComponent<Actor>());

                    // attack
                    other.gameObject.GetComponent<Actor>().TakeDamage(currentDamage);
                }
            }
        }
    }

    public void ResetAttack()
    {
        attacked.Clear();
    }
}
