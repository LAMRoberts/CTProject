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
        if (other.GetComponent<Actor>())
        {
            Actor actor = other.GetComponent<Actor>();

            if (other.tag == "Player")
            {
                if (!sideRoom && !explored)
                {
                    explored = true;

                    player.GetComponent<Profile>().CompletedSideRoom();
                }
            }
            else
            {
                if (player.GetComponent<Actor>().GetSideRoom() == sideRoom)
                {
                    return;
                }
            }

            if (actor.GetSideRoom() == sideRoom)
            {
                other.transform.position = destination;

                other.transform.rotation = transform.rotation;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Actor>())
        {
            Actor actor = other.GetComponent<Actor>();

            actor.SetSideRoom(sideRoom);
        }
    }
}
