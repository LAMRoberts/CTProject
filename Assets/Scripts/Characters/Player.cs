using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public float maxAttackPower = 100.0f;
    public float attackChargeRate = 0.5f;

    private float attackPower = 0.0f;
    private float damageMultiplier = 5.0f;
    private float currentDamage = 0.0f;
    public float staminaCostMultiplier = 0.2f;
    private bool attacking = false;
    private bool charging = false;
    private bool shouldConsumeStamina = false;

    public Canvas hudPrefab;
    public Canvas hud;

    public Elevator inElevator = Elevator.NONE;

    public Vector3 startPosition;

    public Vector3 positionDifference;

    public int playerFloor = 1;

    public GameObject deathMessage;

    private void Start()
    {
        hud = Instantiate(hudPrefab);

        positionDifference = new Vector3(0, 0, 0);

        damageResistance = 0.1f;
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
        else
        {
            transform.position = Vector3.zero;

            StartCoroutine(_Revive());
        }
    }

    private void Attack()
    {
        bool usingController = GetComponent<PlayerMovement>().usingController;

        if (!usingController && (Input.GetMouseButton(0)) || 
            usingController && Input.GetAxis("Right Trigger") != 0.0f)
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
                    attackPower += Time.deltaTime * attackChargeRate; 
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

            // stamina consumption
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

        sc.gameObject.GetComponentInChildren<Sword>().UpdateDamage(attackPower * 10.0f);

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

    IEnumerator _Revive()
    {
        deathMessage.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        deathMessage.SetActive(false);

        Revive();
    }
}
