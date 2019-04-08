using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorPanelController : MonoBehaviour
{
    public GameObject elevator;
    public ElevatorController ec;
    public GameObject player;
    public Player pc;

    private void Start()
    {
        elevator = transform.parent.parent.gameObject;

        ec = elevator.GetComponent<ElevatorController>();

        player = GameObject.FindGameObjectWithTag("Player");

        pc = player.GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            elevator.GetComponent<ElevatorController>().EnterCollider();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (pc.inElevator == ec.whichOne)
            {
                if (other.gameObject.GetComponent<PlayerMovement>().usingController)
                {
                    if (Input.GetButtonDown("X Button"))
                    {
                        ec.ChangeFloors();
                    }
                }
                else
                {
                    if (Input.GetKeyDown("e"))
                    {
                        ec.ChangeFloors();
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            elevator.GetComponent<ElevatorController>().ExitCollider();
        }
    }
}
