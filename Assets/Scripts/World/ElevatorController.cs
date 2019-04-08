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
    NONE        = 0,
    PREVIOUS    = 1,
    NEXT        = 2
}

public class ElevatorController : MonoBehaviour
{
    public GameObject worldInfo;

    public int elevatorFloor = 0;

    public GameObject leftInternalDoor;
    public GameObject rightInternalDoor;

    public Vector3 leftOpenPosition;
    public Vector3 leftClosedPosition;
    public Vector3 rightOpenPosition;
    public Vector3 rightClosedPosition;

    public Elevator whichOne = Elevator.NONE;

    public GameObject player;
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

        player = GameObject.FindGameObjectWithTag("Player");
        hud = player.GetComponent<Player>().hud;

        worldInfo = GameObject.FindGameObjectWithTag("WorldInfo");
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
        if (state == ElevatorState.CLOSED)
        {
            state = ElevatorState.OPENING;
        }

        if (whichOne == Elevator.PREVIOUS)
        {
            //hud.GetComponent<HUDController>().Activate(true);

            player.GetComponent<Player>().inElevator = whichOne;
        }
        else if (whichOne == Elevator.NEXT)
        {
            hud.GetComponent<HUDController>().Activate(true);

            player.GetComponent<Player>().inElevator = whichOne;
        }
    }

    public void ExitCollider()
    {
        if (whichOne == Elevator.PREVIOUS)
        {
            //hud.GetComponent<HUDController>().Activate(false);
        }
        else if (whichOne == Elevator.NEXT)
        {
            hud.GetComponent<HUDController>().Activate(false);
        }
    }

    public bool OpenDoors()
    {
        return true;
    }

    public bool CloseDoors()
    {
        return true;
    }

    public void ChangeFloors()
    {
        if (whichOne == Elevator.NEXT)
        {
            //Debug.Log("Elevator is at floor: " + elevatorFloor + ", Going Down");

            player.GetComponent<Player>().SetLevel(elevatorFloor + 1);

            worldInfo.GetComponent<WorldInfo>().SetLowestFloor(elevatorFloor + 1);

            player.GetComponent<Player>().positionDifference = transform.position - player.transform.position;
        }
        else if (whichOne == Elevator.PREVIOUS)
        {
            player.GetComponent<Player>().SetLevel(elevatorFloor - 1);
        }
        else
        {
            Debug.Log("ElevatorController.whichOne not set");
        }
    }

}
