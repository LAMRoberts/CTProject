﻿using System.Collections;
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

    public Transform playerPosition;

    public int floor = 1;

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
        hud = player.GetComponent<PlayerController>().hud;

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
            hud.GetComponent<HUDController>().PreviousFloor();

            player.GetComponent<PlayerController>().inElevator = whichOne;
        }
        else if (whichOne == Elevator.NEXT)
        {
            hud.GetComponent<HUDController>().NextFloor();

            player.GetComponent<PlayerController>().inElevator = whichOne;
        }
    }

    public void ExitCollider()
    {
        if (whichOne == Elevator.PREVIOUS)
        {
            hud.GetComponent<HUDController>().PreviousFloor();
        }
        else if (whichOne == Elevator.NEXT)
        {
            hud.GetComponent<HUDController>().NextFloor();
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
            player.GetComponent<PlayerController>().SetLevel(floor + 1);

            worldInfo.GetComponent<WorldInfo>().SetLowestFloor(floor + 1);
        }
        else if (whichOne == Elevator.PREVIOUS)
        {
            player.GetComponent<PlayerController>().SetLevel(floor - 1);
        }
        else
        {
            Debug.Log("ElevatorController.whichOne not set");
        }
    }

}
