using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElevatorState
{
    OPEN        = 0,
    OPENING     = 1,
    CLOSED      = 2,
    CLOSING     = 3
}

public enum Elevator
{
    PREVIOUS    = 0,
    NEXT        = 1
}

public class ElevatorController : MonoBehaviour
{
    public Transform playerPosition;

    [SerializeField]
    private int floor = 1;

    public GameObject leftInternalDoor;
    public GameObject rightInternalDoor;

    public Vector3 leftOpenPosition;
    public Vector3 leftClosedPosition;
    public Vector3 rightOpenPosition;
    public Vector3 rightClosedPosition;

    public Elevator whichOne;

    public Canvas hud;

    private int objectsInRange = 0;

    [SerializeField]
    private ElevatorState state = ElevatorState.CLOSED;

    private float timeSpentOpen = 0.0f;
    public float timeToStayOpen = 5.0f;

    void Start()
    {
        leftOpenPosition = leftInternalDoor.transform.position;
        rightOpenPosition = rightInternalDoor.transform.position;

        hud = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().hud;
    }

    void Update()
    {
        switch (state)
        {
            case ElevatorState.CLOSED:
                {
                    if (objectsInRange > 0)
                    {
                        state = ElevatorState.OPENING;
                    }

                    break;
                }
            case ElevatorState.CLOSING:
                {
                    if (CloseDoors())
                    {
                        state = ElevatorState.CLOSED;
                    }

                    break;
                }
            case ElevatorState.OPEN:
                {
                    if (timeSpentOpen < timeToStayOpen)
                    {
                        timeSpentOpen += Time.deltaTime;
                    }
                    else
                    {
                        state = ElevatorState.CLOSING;
                    }

                    break;
                }
            case ElevatorState.OPENING:
                {
                    if (OpenDoors())
                    {
                        state = ElevatorState.OPEN;
                    }

                    break;
                }
        }
    }

    public void EnterCollider()
    {
        objectsInRange++;

        if (state == ElevatorState.CLOSED)
        {
            state = ElevatorState.OPENING;
        }

        if (whichOne == Elevator.PREVIOUS)
        {
            hud.GetComponent<HUDController>().PreviousFloor();
        }
        else if (whichOne == Elevator.NEXT)
        {
            hud.GetComponent<HUDController>().NextFloor();
        }
    }

    public void ExitCollider()
    {
        objectsInRange--;

        if (whichOne == Elevator.PREVIOUS)
        {
            hud.GetComponent<HUDController>().PreviousFloor();
        }
        else if (whichOne == Elevator.NEXT)
        {
            hud.GetComponent<HUDController>().NextFloor();
        }
    }

    bool OpenDoors()
    {
        return true;
    }

    bool CloseDoors()
    {
        return true;
    }
}
