using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    IDLE,
    COMBAT,
    DEAD
}

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private MeshRenderer r;
    private SwordController sc;

    private State state = State.IDLE;

    public float viewRange = 10.0f;
    public float combatRange = 10.0f;
    public float personalSpace = 4.0f;

    private float distanceToPlayer;
    private Vector3 targetPosition;

    public float walkingSpeed = 10.0f;
    public float runningSpeed = 20.0f;

    public float maxAttackPower = 100.0f;
    public float attackChargeRate = 0.5f;

    public float speed = 10.0f;

    private float attackPower = 0.0f;

    private bool playerInView = false;
    private Vector3 lastKnownPosition = new Vector3(0, 0, 0);

    [SerializeField]
    private float timeSinceLastAttack = 0.0f;
    public float TimeBetweenAttacks = 3.0f;

    [SerializeField]
    private bool attacking = false;
    [SerializeField]
    private bool charging = false;
    [SerializeField]
    private bool committed = false;
    [SerializeField]
    private bool attackReady = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        sc = gameObject.GetComponentInChildren<SwordController>();

        r = GetComponent<MeshRenderer>();
        r.material.color = Color.green;
    }

    private void Update()
    {
        distanceToPlayer = (player.transform.position - transform.position).magnitude;
        targetPosition = new Vector3(lastKnownPosition.x, 0.0f, lastKnownPosition.z);

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
                    r.material.color = Color.yellow;

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
                    r.material.color = Color.red;

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
                if (hit.distance < viewRange)
                {
                    Debug.DrawRay(transform.position, (player.transform.position - transform.position) * hit.distance, Color.green);

                    if (state != State.COMBAT)
                    {
                        state = State.COMBAT;
                    }

                    playerInView = true;
                    lastKnownPosition = player.transform.position;
                }
                else
                {
                    Debug.DrawRay(transform.position, (player.transform.position - transform.position) * hit.distance, Color.yellow);

                    playerInView = false;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, (player.transform.position - transform.position) * hit.distance, Color.red);
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
}
