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

    private bool isABoss = false;
    private float difficulty = 1.0f;
    private int playersKilled = 0;

    public Transform healthBarPoint;
    private int healthBarNumber;
    public float viewRange = 3.0f;
    public float combatRange = 10.0f;
    public float personalSpace = 2.0f;

    public float maxMovementSpeed = 10.0f;
    public float currentMovementSpeed = 0.0f;
    public float rotationSpeed = 2.0f;
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

    public PhysicMaterial physicsMaterial;
    private bool droppedSword = false;

    private bool playerInfront = false;
    private bool playerInSight = false;

    private float interestTimer = 0.0f;

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

        if (dead)
        {
            state = State.DEAD;
        }

        switch (state)
        {
            case State.IDLE:
                {
                    r.material.color = Color.green;

                    TargetInView();

                    break;
                }
            case State.COMBAT:
                {
                    TargetInView();

                    if (!committed)
                    {
                        Movement();
                    }

                    if (distanceToPlayer < personalSpace && !attacking && timeSinceLastAttack > TimeBetweenAttacks)
                    {
                        attackReady = true;
                    }

                    if (attackReady && playerInfront)
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
                    if (!droppedSword)
                    {
                        r.material.color = Color.black;

                        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                        GetComponent<Rigidbody>().mass = 1.0f;

                        GetComponent<CapsuleCollider>().material = physicsMaterial;

                        DropSword();

                        droppedSword = true;

                        player.GetComponent<Profile>().KilledEnemy(new EnemyInfo(isABoss, difficulty, playersKilled));
                    }

                    break;
                }
        }
    }

    void TargetInView()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

                if (hit.distance < viewRange)
                {                    
                    Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
                    float dot = Vector3.Dot(dirToPlayer, transform.forward);
                    Debug.Log(dot);

                    if (dot > 0.5f)
                    {
                        if (!playerInfront)
                        {
                            playerInfront = true;
                        }
                    }
                    else
                    {
                        if(playerInfront)
                        {
                            playerInfront = false;
                        }
                    }

                    if (playerInfront || distanceToPlayer < personalSpace)
                    {
                        //Debug.DrawRay(transform.position, (player.transform.position - transform.position) * hit.distance, Color.green);
                        r.material.color = Color.red;

                        if (state != State.COMBAT)
                        {
                            state = State.COMBAT;
                        }

                        lastKnownPosition = player.transform.position;
                    }

                    playerInSight = true;
                }
                else
                {
                    //Debug.DrawRay(transform.position, (player.transform.position - transform.position) * hit.distance, Color.yellow);
                    r.material.color = Color.yellow;

                    if (playerInSight)
                    {
                        playerInSight = false;
                    }
                }
            }
            else
            {
                if (playerInSight)
                {
                    playerInSight = false;
                }
                //Debug.DrawRay(transform.position, (player.transform.position - transform.position) * hit.distance, Color.red);
            }
        }
    }

    void LooseInterest()
    {
        if (state == State.COMBAT && distanceToPlayer > viewRange)
        {
            interestTimer += Time.deltaTime;
            // countdown timer for interest
        }
        else
        {
            interestTimer = 0.0f;
        }

        if (interestTimer > 5.0f)
        {
            state = State.IDLE;
        }
    }

    void Movement()
    {
        if (state == State.COMBAT)
        {
            // rotate towards player
            if (!committed)
            {
                Vector3 targetDir = player.transform.position - transform.position;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotationSpeed * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDir);
            }
            
            // move towards player
            if (playerInfront)
            {
                if (distanceToPlayer > combatRange)
                {
                    maxMovementSpeed = runningSpeed;

                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * maxMovementSpeed);
                }
                else if (distanceToPlayer < combatRange && distanceToPlayer > personalSpace)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * maxMovementSpeed);
                }
                else if (distanceToPlayer < personalSpace)
                {
                    maxMovementSpeed = walkingSpeed;

                    // walk away from player
                    // transform.position = Vector3.MoveTowards(transform.position, transform.position - targetPosition, Time.deltaTime * speed);
                }
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

    void DropSword()
    {
        GameObject sword = GetComponentInChildren<SwordController>().gameObject;

        Rigidbody rb = sword.AddComponent<Rigidbody>();

        rb.useGravity = true;

        rb.mass = 10.0f;

        rb.drag = 4.0f;

        rb.angularDrag = 1.0f;

        sword.GetComponent<CapsuleCollider>().enabled = true;

        sword.transform.SetParent(null);
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
