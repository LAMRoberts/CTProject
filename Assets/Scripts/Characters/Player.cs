using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public float maxAttackPower = 100.0f;
    public float attackChargeRate = 0.5f;

    private float attackPower = 0.0f;
    public float staminaCostMultiplier = 0.2f;
    private bool attacking = false;
    private bool charging = false;
    private bool shouldConsumeStamina = false;

    public Canvas hudPrefab;
    public Canvas hud;

    public Elevator inElevator = Elevator.NONE;

    public Vector3 positionDifference;

    public int playerFloor = 1;

    private void Start()
    {
        hud = Instantiate(hudPrefab);

        positionDifference = new Vector3(0, 0, 0);
    }

    private void LateUpdate()
    {
        if (!dead)
        {
            if (Input.GetKeyDown("h"))
            {
                TakeDamage(15);
            }

            Attack();
        }
    }

    private void Attack()
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
                // add attack power
                if (attackPower < maxAttackPower)
                {
                    attackPower += attackChargeRate;
                }

                // consume stamina
                if (attackPower != 0.0f)
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

                    if (attackPower < 20.0f)
                    {
                        GetComponent<PlayerMovement>().ConsumeStamina((4.0f) * staminaCostMultiplier);
                    }
                    else
                    {
                        GetComponent<PlayerMovement>().ConsumeStamina((attackPower * 0.2f) * staminaCostMultiplier);
                    }
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

        sc.gameObject.GetComponentInChildren<Sword>().UpdateDamage(100.0f + attackPower);

        attacking = sc.UpdateSword(attacking, charging);
    }

    void SetPlayerInElevator(Elevator value)
    {
        inElevator = value;
    }

    public void SetLevel(int floorNumber)
    {
        playerFloor = floorNumber;
    }
}
