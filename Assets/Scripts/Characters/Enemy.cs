using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    IDLE,
    COMBAT,
    DEAD
}

public class Enemy : Actor
{
    private GameObject player;
    private MeshRenderer r;
    private SwordController sc;
    private GameObject hud;
    private HUDController hc;

    private State state = State.IDLE;

    public Transform healthBarPoint;
    private int healthBarNumber;
    public float viewRange = 10.0f;
    public float combatRange = 10.0f;
    public float personalSpace = 4.0f;

    public float speed = 10.0f;
    public float walkingSpeed = 10.0f;
    public float runningSpeed = 20.0f;

    private Vector3 targetPosition;
    private Vector3 lastKnownPosition = new Vector3(0, 0, 0);
    private float distanceToPlayer;

    public float TimeBetweenAttacks = 3.0f;
    public float attackChargeRate = 0.5f;
    public float maxAttackPower = 100.0f;

    private float attackPower = 0.0f;
    private float timeSinceLastAttack = 0.0f;
    private bool attacking = false;
    private bool charging = false;
    private bool committed = false;
    private bool attackReady = false;

    private void Start()
    {
        // get player
        player = GameObject.FindGameObjectWithTag("Player");

        // get sword controller
        sc = GetComponentInChildren<SwordController>();

        // set material colour
        r = GetComponent<MeshRenderer>();
        r.material.color = Color.green;

        // set enemy health bar
        hud = GameObject.FindGameObjectWithTag("HUD");
        hc = hud.GetComponent<HUDController>();

        hc.AddEnemyHealthBar(gameObject);
    }

    private void Update()
    {
        distanceToPlayer = (player.transform.position - transform.position).magnitude;
        targetPosition = new Vector3(lastKnownPosition.x, transform.position.y, lastKnownPosition.z);

        switch (state)
        {
            case State.IDLE:
                {
                    r.material.color = Color.green;

                    TargetInRange();

                    break;
                }
            case State.COMBAT:
                {
                    TargetInRange();

                    if (!committed)
                    {
                        Movement();
                    }

                    if (distanceToPlayer < personalSpace && !attacking && timeSinceLastAttack > TimeBetweenAttacks)
                    {
                        attackReady = true;
                    }

                    if (attackReady)
                    {
                        Attack();
                    }

                    if (!attacking)
                    {
                        if (timeSinceLastAttack < TimeBetweenAttacks)
                        {
                            timeSinceLastAttack += Time.deltaTime;
                        }
                    }

                    break;
                }
            case State.DEAD:
                {
                    r.material.color = Color.black;

                    break;
                }
        }
    }

    void TargetInRange()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

                if (hit.distance < viewRange)
                {
                    //Debug.DrawRay(transform.position, (player.transform.position - transform.position) * hit.distance, Color.green);
                    r.material.color = Color.red;

                    if (state != State.COMBAT)
                    {
                        state = State.COMBAT;
                    }

                    lastKnownPosition = player.transform.position;
                }
                else
                {
                    //Debug.DrawRay(transform.position, (player.transform.position - transform.position) * hit.distance, Color.yellow);
                    r.material.color = Color.yellow;
                }
            }
            else
            {
                //Debug.DrawRay(transform.position, (player.transform.position - transform.position) * hit.distance, Color.red);
            }
        }
    }

    void Movement()
    {
        if (state == State.COMBAT)
        {
            if (!committed)
            {
                Vector3 difference = targetPosition - transform.position;
                float rotationY = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
            }          

            if (distanceToPlayer > combatRange)
            {
                speed = runningSpeed;

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
            }
            else if (distanceToPlayer < combatRange && distanceToPlayer > personalSpace)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
            }
            else if (distanceToPlayer < personalSpace)
            {
                speed = walkingSpeed;

                // walk away from player
                // transform.position = Vector3.MoveTowards(transform.position, transform.position - targetPosition, Time.deltaTime * speed);
            }
        }
    }

    void Attack()
    {
        if (!attacking && attackPower != 0.0f)
        {
            attackPower = 0.0f;
        }

        if (!attacking)
        {
            attacking = true;
            charging = true;
        }

        if (attacking && charging)
        {
            if (attackPower <= maxAttackPower)
            {
                attackPower += attackChargeRate;
            }
            else
            {
                charging = false;

                committed = true;
            }
        }

        UpdateSword();
    }

    void UpdateSword()
    {
        sc.UpdateColour(attackPower / 100);

        attacking = sc.UpdateSword(attacking, charging);

        // resetting after attack
        if (!attacking)
        {
            timeSinceLastAttack = 0.0f;

            attackReady = false;

            committed = false;

            attackPower = 0.0f;
        }
    }

    private int GetUINumber()
    {
        return healthBarNumber;
    }

    private void SetUINumber(int no)
    {
        healthBarNumber = no;
    }
}
