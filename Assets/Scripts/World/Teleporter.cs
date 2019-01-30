using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform portal;
    public Vector3 destination;
    public GameObject player;

    public bool sideRoom = false;

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
        if (other.GetComponent<ActorTeleport>())
        {
            Debug.Log("Actor Teleporting");

            ActorTeleport at = other.GetComponent<ActorTeleport>();

            if (at.GetSideRoom() == sideRoom)
            {
                other.transform.position = destination;

                other.transform.rotation = transform.rotation;
            }

            if (other.tag == "Player")
            {
                if (!sideRoom && !explored)
                {
                    explored = true;

                    player.GetComponent<Profile>().CompletedSideRoom();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ActorTeleport>())
        {
            ActorTeleport at = other.GetComponent<ActorTeleport>();

            at.SetSideRoom(sideRoom);
        }
    }
}
