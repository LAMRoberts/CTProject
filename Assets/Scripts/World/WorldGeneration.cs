using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NodeInfo
{
    public Vector3 previousPosition;
    public Vector3 nextPosition;

    // constructor
    public NodeInfo(Vector3 prevPos, Vector3 nextPos)
    {
        previousPosition = prevPos;
        nextPosition = nextPos;
    }
}

public struct SideRoomInfo
{
    public GameObject node;
    public bool isToEast;

    // constructor
    public SideRoomInfo(GameObject worldNode, bool eastward)
    {
        node = worldNode;
        isToEast = eastward;
    }
}

public class WorldGeneration : MonoBehaviour
{
    public int LevelLength = 10;

    private List<GameObject> worldNodes;

    public int NoOfSideRooms = 1;

    private List<SideRoomInfo> potentialSideRooms;

    public GameObject worldNode;
    public GameObject roomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject startRoomPrefab;
    public GameObject elevatorPrefab;
    public GameObject sideRoomPrefab;
    public GameObject wallPrefab;

    private GameObject startElevatorRoom;
    private GameObject bossRoom;
    private GameObject startElevator;
    private GameObject endElevator;


    void Start()
    {
        worldNodes = new List<GameObject>();

        potentialSideRooms = new List<SideRoomInfo>();

        GenerateWorld();
	}

    void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            DestroyWorld();

            GenerateWorld();
        }
    }
    
    // remove all room gameobjects
    void DestroyWorld()
    {
        // delete world
        foreach (GameObject node in worldNodes)
        {          
            Destroy(node);
        }  
        
        // clear list
        worldNodes.Clear();

        potentialSideRooms.Clear();
    }

    // start world generation
    void GenerateWorld()
    {
        // fill world
        GenerateNodes();

        // populate room nodes
        FillNodes();
    }

    // set world node positions
    void GenerateNodes()
    {
        NodeInfo newPos = new NodeInfo(new Vector3(0, 0, 0), new Vector3(0, 0, 0));

	    for (int i = 0; i < LevelLength; i++)
        {
            GameObject nextWorldNode = Instantiate(worldNode, transform);
                                              
            worldNodes.Add(nextWorldNode);

            nextWorldNode.GetComponent<NodeController>().previous = new Vector3(0, 0, 0);

            if (i != 0)
            {
                // find the next position
                newPos = FindNextPosition(newPos);

                // set direction to previous node
                nextWorldNode.GetComponent<NodeController>().previous = newPos.previousPosition;

                // set previous nodes next position
                worldNodes[i-1].GetComponent<NodeController>().next = newPos.nextPosition;
            }
            
            nextWorldNode.transform.position = newPos.nextPosition;
        }
    }

    // find next world node position
    NodeInfo FindNextPosition(NodeInfo info)
    {
        int loopBreaker = 0;
        while (loopBreaker < 100)
        {
            int direction = (int)Random.Range(0.0f, 2.9999f);
                        
            switch (direction)
            {
                case 0: // NORTH
                    {
                        return new NodeInfo(info.nextPosition, info.nextPosition + new Vector3(0, 0, 10));
                    }
                case 1: // WEST
                    {
                        if (!Equals(info.previousPosition, info.nextPosition + new Vector3(-10, 0, 0)))
                        {
                            return new NodeInfo(info.nextPosition, info.nextPosition + new Vector3(-10, 0, 0));
                        }
                        else
                        {
                            break;
                        }
                    }
                case 2: // EAST
                    {
                        if (!Equals(info.previousPosition, info.nextPosition + new Vector3(10, 0, 0)))
                        {
                            return new NodeInfo(info.nextPosition, info.nextPosition + new Vector3(10, 0, 0));
                        }
                        else
                        {
                            break;
                        }
                    }
            }
            
            loopBreaker++;
        }

        return new NodeInfo(info.nextPosition, info.nextPosition + new Vector3(10, 0, 0));
    }

    // instanciate rooms in world
    void FillNodes()
    {
        int count = 1;
        foreach (GameObject node in worldNodes)
        {
            bool north = false;
            bool east = false;
            bool south = false;
            bool west = false;
            
            if (count == 1)
            {
                startElevatorRoom = Instantiate(startRoomPrefab, node.transform);

                startElevator = Instantiate(elevatorPrefab);

                startElevator.transform.position = startElevatorRoom.GetComponent<ElevatorRoomController>().elevatorPosition.position;

                startElevator.GetComponent<ElevatorController>().whichOne = Elevator.PREVIOUS;



                south = true;
            }
            else if (count == worldNodes.Count)
            {
                bossRoom = Instantiate(bossRoomPrefab, node.transform);

                endElevator = Instantiate(elevatorPrefab);

                endElevator.transform.position = bossRoom.GetComponent<ElevatorRoomController>().elevatorPosition.position;

                endElevator.transform.Rotate(new Vector3(0, 180, 0));

                endElevator.GetComponent<ElevatorController>().whichOne = Elevator.NEXT;

                north = true;
            }
            else
            {
                // instantiate normal room
                Instantiate(roomPrefab, node.transform);
            }

            // Check rooms for potential wall positions
            foreach (GameObject room in worldNodes)
            {
                // if a room is to the north
                if (Equals(room.transform.position, node.transform.position + new Vector3(0, 0, 10)))
                {
                    north = true;
                }
                // if a room is to the south
                if (Equals(room.transform.position, node.transform.position - new Vector3(0, 0, 10)))
                {
                    south = true;
                }
                // if a room is to the east
                if (Equals(room.transform.position, node.transform.position + new Vector3(10, 0, 0)))
                {
                    east = true;
                }
                // if a room is to the west
                if (Equals(room.transform.position, node.transform.position - new Vector3(10, 0, 0)))
                {
                    west = true;
                }
            }

            // make list of potential side rooms
            // if theres a room to north and a room to the south
            if (north == true && south == true)
            {
                // if theres no room to the east
                if (east == false)
                {
                    SideRoomInfo sideRoomInfo = new SideRoomInfo(node, true);

                    potentialSideRooms.Add(sideRoomInfo);

                    east = true;

                    GameObject sideRoom = Instantiate(sideRoomPrefab, node.transform);

                    sideRoom.transform.position = node.transform.position + new Vector3(10, 0, 0);
                }
                // if theres no room to the west
                if (west == false)
                {
                    SideRoomInfo sideRoomInfo = new SideRoomInfo(node, false);

                    potentialSideRooms.Add(sideRoomInfo);

                    west = true;

                    GameObject sideRoom = Instantiate(sideRoomPrefab, node.transform);

                    sideRoom.transform.position = node.transform.position + new Vector3(-10, 0, 0);

                    sideRoom.transform.Rotate(new Vector3(0, 180, 0), Space.Self);
                }
            }

            // instantiate walls
            if (!north)
            {
                Instantiate(wallPrefab, node.GetComponent<NodeController>().northWall);
            }
            if (!east)
            {
                Instantiate(wallPrefab, node.GetComponent<NodeController>().eastWall);
            }
            if (!south)
            {
                Instantiate(wallPrefab, node.GetComponent<NodeController>().southWall);
            }
            if (!west)
            {
                Instantiate(wallPrefab, node.GetComponent<NodeController>().westWall);
            }

            count++;
        }
    }
}
