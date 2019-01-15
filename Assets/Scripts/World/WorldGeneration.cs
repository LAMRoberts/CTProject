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
    public int generatedFloor = 1;

    public int levelLength = 10;

    private List<GameObject> worldNodes;
    private List<SideRoomInfo> potentialSideRooms;
    private List<SideRoomInfo> sideRooms;

    public int maxSideRooms = 1;
       
    public GameObject worldNode;
    public GameObject roomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject startRoomPrefab;
    public GameObject elevatorPrefab;
    public GameObject sideRoomPrefab;
    public GameObject wallPrefab;


    private GameObject player;
    private PlayerController pc;
    private GameObject startElevatorRoom;
    private GameObject bossRoom;
    public GameObject startElevator;
    public GameObject endElevator;

    private GameObject worldInfo;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        worldInfo = GameObject.FindGameObjectWithTag("WorldInfo");

        maxSideRooms = player.GetComponent<Profile>().maxSideRooms;

        generatedFloor = pc.playerFloor;

        worldNodes = new List<GameObject>();
        potentialSideRooms = new List<SideRoomInfo>();
        sideRooms = new List<SideRoomInfo>();

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
        
        // clear lists
        worldNodes.Clear();
        potentialSideRooms.Clear();
        sideRooms.Clear();
    }

    // start world generation
    void GenerateWorld()
    {
        // fill world
        GenerateNodes();

        // populate room nodes with rooms
        FillNodes();

        // find potential wall positions
        FindWallPositions();

        // find potential side rooms and generate them
        GenerateSideRooms();

        // populate rooms with walls
        GenerateWalls();
    }

    // set world node positions
    void GenerateNodes()
    {
        NodeInfo newPos = new NodeInfo(new Vector3(0, 0, 0), transform.position);

	    for (int i = 0; i < levelLength; i++)
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

    // instantiate rooms in world
    void FillNodes()
    {
        int roomNumber = 1;

        foreach (GameObject node in worldNodes)
        {
            NodeController nc = node.GetComponent<NodeController>();
            
            if (roomNumber == 1)
            {
                // starting elevator room
                startElevatorRoom = Instantiate(startRoomPrefab, node.transform);

                // start elevator
                if (!startElevator)
                {
                    startElevator = Instantiate(elevatorPrefab);

                    startElevator.GetComponent<ElevatorController>().whichOne = Elevator.PREVIOUS;

                    startElevator.gameObject.tag = "StartElevator";
                }

                startElevator = GameObject.FindGameObjectWithTag("StartElevator");

                startElevator.GetComponent<ElevatorController>().elevatorFloor = generatedFloor;

                startElevator.transform.position = startElevatorRoom.GetComponent<ElevatorRoomController>().elevatorPosition.position;

                // set walls to open
                nc.south = true;
            }
            else if (roomNumber == worldNodes.Count)
            {
                // boss room
                bossRoom = Instantiate(bossRoomPrefab, node.transform);

                // end elevator
                if (!endElevator)
                {
                    endElevator = Instantiate(elevatorPrefab);

                    endElevator.transform.Rotate(new Vector3(0, 180, 0));

                    endElevator.GetComponent<ElevatorController>().whichOne = Elevator.NEXT;

                    endElevator.gameObject.tag = "EndElevator";
                }

                // after floor one dont make a new elevator
                endElevator = GameObject.FindGameObjectWithTag("EndElevator");

                endElevator.GetComponent<ElevatorController>().elevatorFloor = generatedFloor;

                endElevator.transform.position = bossRoom.GetComponent<ElevatorRoomController>().elevatorPosition.position;

                // set walls to open
                nc.north = true;
            }
            else
            {
                // instantiate normal room
                Instantiate(roomPrefab, node.transform);
            }

            roomNumber++;
        }
        
        if (worldInfo.GetComponent<WorldInfo>().GetLowestFloor() != 1)
        {
            player.transform.position = new Vector3(startElevator.transform.position.x + pc.positionDifference.x, startElevator.transform.position.y + 2, startElevator.transform.position.z + pc.positionDifference.z);
        }
    }
    
    private void FindWallPositions()
    {
        foreach (GameObject node in worldNodes)
        {
            NodeController nc = node.GetComponent<NodeController>();

            // Check rooms for potential wall positions
            foreach (GameObject room in worldNodes)
            {
                // if a room is to the north
                if (Equals(room.transform.position, node.transform.position + new Vector3(0, 0, 10)))
                {
                    nc.north = true;
                }
                // if a room is to the south
                if (Equals(room.transform.position, node.transform.position - new Vector3(0, 0, 10)))
                {
                    nc.south = true;
                }
                // if a room is to the east
                if (Equals(room.transform.position, node.transform.position + new Vector3(10, 0, 0)))
                {
                    nc.east = true;
                }
                // if a room is to the west
                if (Equals(room.transform.position, node.transform.position - new Vector3(10, 0, 0)))
                {
                    nc.west = true;
                }
            }
        }
    }

    private void GenerateSideRooms()
    {
        // find potential side rooms
        foreach (GameObject node in worldNodes)
        {
            NodeController nc = node.GetComponent<NodeController>();

            if (nc.north == true && nc.south == true)
            {
                // if theres no room to the east
                if (nc.east == false)
                {
                    SideRoomInfo sideRoomInfo = new SideRoomInfo(node, true);

                    potentialSideRooms.Add(sideRoomInfo);
                }
                // if theres no room to the west
                if (nc.west == false)
                {
                    SideRoomInfo sideRoomInfo = new SideRoomInfo(node, false);

                    potentialSideRooms.Add(sideRoomInfo);                
                }
            }
        }

        int sideRoomSpacing = potentialSideRooms.Count / maxSideRooms;

        for (int index = 0; index < maxSideRooms; index++)
        {
            // find next floor room to place a side room in
            int rng = Random.Range((index * sideRoomSpacing), ((index + 1) * sideRoomSpacing));

            // add room to side rooms list
            sideRooms.Add(potentialSideRooms[rng]);            
        }

        foreach (SideRoomInfo info in sideRooms)
        {
            NodeController nc = info.node.GetComponent<NodeController>();

            if (info.isToEast)
            {
                nc.east = true;

                GameObject sideRoom = Instantiate(sideRoomPrefab, info.node.transform);

                sideRoom.transform.position = info.node.transform.position + new Vector3(10, 0, 0);
            }
            else
            {
                nc.west = true;

                GameObject sideRoom = Instantiate(sideRoomPrefab, info.node.transform);

                sideRoom.transform.position = info.node.transform.position + new Vector3(-10, 0, 0);

                sideRoom.transform.Rotate(new Vector3(0, 180, 0), Space.Self);
            }
        }
    }

    private void GenerateWalls()
    {
        foreach (GameObject node in worldNodes)
        {
            NodeController nc = node.GetComponent<NodeController>();

            // instantiate walls
            if (!nc.north)
            {
                Instantiate(wallPrefab, node.GetComponent<NodeController>().northWall);
            }
            if (!nc.east)
            {
                Instantiate(wallPrefab, node.GetComponent<NodeController>().eastWall);
            }
            if (!nc.south)
            {
                Instantiate(wallPrefab, node.GetComponent<NodeController>().southWall);
            }
            if (!nc.west)
            {
                Instantiate(wallPrefab, node.GetComponent<NodeController>().westWall);
            }
        }
    }
}
