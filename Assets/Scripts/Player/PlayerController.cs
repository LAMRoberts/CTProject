﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float maxAttackPower = 100.0f;
    public float attackChargeRate = 0.5f;

    public int health = 100;

    private float attackPower = 0.0f;
    public float staminaCostMultiplier = 0.2f;
    private bool attacking = false;
    private bool charging = false;
    private bool shouldConsumeStamina = false;
	
	void LateUpdate ()
    {
        if (Input.GetKeyDown("h"))
        {
            GetHit(15);
        }

        Attack();
    }

    void Attack()
    {
        if (Input.GetMouseButton(0))
        {
            if (!attacking)
            {
                attacking = true;
                charging = true;
            }

            if (charging)
            {
                if (attackPower <= maxAttackPower)
                {
                    attackPower += attackChargeRate;
                }

                if (attackPower > (maxAttackPower * 0.5f))
                {
                    shouldConsumeStamina = true;
                }
            }
        }
        else
        {
            charging = false;

            if (attackPower != 0.0f)
            {
                if (shouldConsumeStamina)
                {
                    shouldConsumeStamina = false;

                    GetComponent<PlayerMovement>().ConsumeStamina((attackPower * 0.2f) * staminaCostMultiplier);
                }

                if (!attacking)
                {
                    attackPower = 0.0f;
                }
            }
        }

        UpdateSword();
    }

    void UpdateSword()
    {
        SwordController sc = gameObject.GetComponentInChildren<SwordController>();

        sc.UpdateColour(attackPower / 100);

        attacking = sc.UpdateSword(attacking, charging);
    }

    void GetHit(int value)
    {
        if (health >= value)
        {
            health -= value;
        }
        else
        {
            health = 0;
        }
    }
}
