using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform portal;
    public Vector3 destination;
    public GameObject player;

    public bool sideRoom = false;
    public bool online = false;

    private bool explored = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetDestination(Vector3 position)
    {
        destination = position;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (online)
        {
            if (sideRoom)
            {
                player.GetComponent<PlayerController>().inSideRoom = false;
            }
            else
            {
                player.GetComponent<PlayerController>().inSideRoom = true;
            }

            player.transform.position = destination;

            player.transform.rotation = transform.rotation;

            online = false;
        }

        if (!sideRoom && !explored)
        {
            explored = true;

            player.GetComponent<Profile>().CompletedSideRoom();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (sideRoom)
        {
            if (player.GetComponent<PlayerController>().inSideRoom)
            {
                online = true;
            }
        }
        else
        {
            if (!player.GetComponent<PlayerController>().inSideRoom)
            {
                online = true;
            }
        }
    }
}
