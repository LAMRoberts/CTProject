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

    private int objectsInRange = 0;

    [SerializeField]
    private ElevatorState state = ElevatorState.CLOSED;

    private float timeSpentOpen = 0.0f;
    public float timeToStayOpen = 5.0f;

    void Start()
    {
        leftOpenPosition = leftInternalDoor.transform.position;
        rightOpenPosition = rightInternalDoor.transform.position;
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
    }

    public void ExitCollider()
    {
        objectsInRange--;
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
