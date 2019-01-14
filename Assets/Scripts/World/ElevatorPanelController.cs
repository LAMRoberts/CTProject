using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorPanelController : MonoBehaviour
{
    public GameObject elevator;

    private void Start()
    {
        elevator = transform.parent.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            elevator.GetComponent<ElevatorController>().EnterCollider();
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
