using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public GameObject player;
    public Canvas hud;
    public ParticleSystem goodies;

    bool opened = false;
    bool particles = false;

    public float speed;

    public Transform hinge;
    public Transform closed;
    public Transform open;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        hud = player.GetComponent<Player>().hud;
    }

    private void Update()
    {
        if (opened)
        {
            Vector3 newDir = Vector3.RotateTowards(hinge.forward, open.position - hinge.position, speed * Time.deltaTime, 0.0f);

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
            hud.GetComponent<HUDController>().Activate(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown("e"))
            {
                if (!opened)
                {
                    opened = true;

                    hud.GetComponent<HUDController>().Activate(false);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !opened)
        {
            hud.GetComponent<HUDController>().Activate(false);
        }
    }

    IEnumerator _PlayParticles()
    {
        if (!particles)
        {
            yield return new WaitForSeconds(0.5f);

            goodies.Play();

            particles = true;
        }
    }
}
