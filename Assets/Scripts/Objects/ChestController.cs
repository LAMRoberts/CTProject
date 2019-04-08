using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public GameObject player;
    public Canvas hud;
    public ParticleSystem chestParticles;

    [SerializeField]
    private List<GameObject> coins;
    public float explosionForce = 0.0f;
    public Transform explosionPosition;
    public float explosionRadius = 0.0f;
    public float explosionLift = 0.0f;

    bool opened = false;
    bool particles = false;

    public float explosionDelay = 0.0f;

    public float openingSpeed;

    public Transform hinge;
    public Transform closed;
    public Transform open;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        hud = player.GetComponent<Player>().hud;

        coins = new List<GameObject>();

        Rigidbody[] rb = GetComponentsInChildren<Rigidbody>();

        foreach(Rigidbody rigidbody in rb)
        {
            if (rigidbody.gameObject.tag == "Coin")
            {
                coins.Add(rigidbody.gameObject);
            }
        }
    }

    private void Update()
    {
        if (opened)
        {
            Vector3 newDir = Vector3.RotateTowards(hinge.forward, open.position - hinge.position, openingSpeed * Time.deltaTime, 0.0f);

            hinge.rotation = Quaternion.LookRotation(newDir);

            StartCoroutine("_PlayParticles");

            if (particles)
            {
                StopCoroutine("_PlayParticles");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !opened)
        {
            if (hud != null)
            {
                hud.GetComponent<HUDController>().Activate(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.GetComponent<PlayerMovement>().usingController)
            {
                if (Input.GetButtonDown("X Button"))
                {
                    if (!opened)
                    {
                        opened = true;

                        if (hud != null)
                        {
                            hud.GetComponent<HUDController>().Activate(false);
                        }
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown("e"))
                {
                    if (!opened)
                    {
                        opened = true;

                        if (hud != null)
                        {
                            hud.GetComponent<HUDController>().Activate(false);
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !opened)
        {
            if (hud != null)
            {
                hud.GetComponent<HUDController>().Activate(false);
            }
        }
    }

    IEnumerator _PlayParticles()
    {
        if (!particles)
        {
            yield return new WaitForSeconds(explosionDelay);

            foreach (GameObject coin in coins)
            {
                coin.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, explosionPosition.position, explosionRadius, explosionLift);
                //Debug.Log("Boom");
            }

            particles = true;
        }
    }
}