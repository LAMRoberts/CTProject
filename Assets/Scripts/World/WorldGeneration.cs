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
    public GameObject worldNode;
    public bool toEast;
    public List<GameObject> nodes;
    public float chestProbability;

    // constructor
    public SideRoomInfo(GameObject wNode, bool eastward, float chest)
    {
        worldNode = wNode;
        toEast = eastward;
        nodes = new List<GameObject>();
        chestProbability = chest;
    }
}

// totally unused, completely forgot how i was gonna use this
public struct PortalInfo
{
    public GameObject mainPortal;
    public GameObject sidePortal;
    public GameObject chest;

    // constructor
    public PortalInfo(GameObject main, GameObject side, GameObject chestObj)
    {
        mainPortal = main;
        sidePortal = side;
        chest = chestObj;
    }
}

public class WorldGeneration : MonoBehaviour
{
    public int generatedFloor = 1;

    public int levelLength = 10;
    public int maxSideRooms = 1;

    public int sideRoomLength = 10;

    private List<GameObject> worldNodes;
    private List<SideRoomInfo> potentialSideRooms;
    private List<SideRoomInfo> sideRooms;
    private List<PortalInfo> portals;
              
    public GameObject worldNodePrefab;
    public GameObject roomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject startRoomPrefab;
    public GameObject elevatorPrefab;
    public GameObject teleporterRoomPrefab;
    public GameObject wallPrefab;
    public GameObject chestPrefab;
    
    private GameObject player;
    private Player pc;
    private Profile profile;

    private GameObject startElevatorRoom;
    public GameObject startElevator;

    private GameObject bossRoom;
    public GameObject endElevator;

    private GameObject worldInfo;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<Player>();
        profile = player.GetComponent<Profile>();
        worldInfo = GameObject.FindGameObjectWithTag("WorldInfo");

        profile.SetExplorer();

        levelLength = (int)profile.floorLength;
        maxSideRooms = (int)profile.sideRoomCount;

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
        GenerateWorldNodes();

        // populate room nodes with rooms
        FillNodes();

        // find potential wall positions in world rooms
        FindWorldRoomWallPositions();

        // find potential side rooms and generate them
        GenerateSideRooms();

        // fill side rooms
        FillSideRooms();

        // find potential wall positions in side rooms

        FindSideRoomWallPositions();
               
        // populate rooms with walls
        GenerateWalls();
    }

    // set world node positions
    void GenerateWorldNodes()
    {
        NodeInfo newPos = new NodeInfo(new Vector3(0, 0, 0), transform.position);

	    for (int i = 0; i < levelLength; i++)
        {
            GameObject nextWorldNode = Instantiate(worldNodePrefab, transform);
                                              
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

                startElevator = GameObject.FindGameObjectWithTag("StartElevator");

                // start elevator
                if (!startElevator)
                {
                    startElevator = Instantiate(elevatorPrefab);

                    startElevator.GetComponent<ElevatorController>().whichOne = Elevator.PREVIOUS;

                    startElevator.gameObject.tag = "StartElevator";
                }

                startElevator.GetComponent<ElevatorController>().elevatorFloor = generatedFloor;

                startElevator.transform.position = startElevatorRoom.GetComponent<ElevatorRoomController>().elevatorPosition.position;

                // set walls to open
                nc.south = true;
            }
            else if (roomNumber == worldNodes.Count)
            {
                // boss room
                bossRoom = Instantiate(bossRoomPrefab, node.transform);

                endElevator = GameObject.FindGameObjectWithTag("EndElevator");

                // end elevator
                if (!endElevator)
                {
                    endElevator = Instantiate(elevatorPrefab);

                    endElevator.transform.Rotate(new Vector3(0, 180, 0));

                    endElevator.GetComponent<ElevatorController>().whichOne = Elevator.NEXT;

                    endElevator.gameObject.tag = "EndElevator";
                }

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
    
    private void FindWorldRoomWallPositions()
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

    private void FindSideRoomWallPositions()
    {
        foreach (SideRoomInfo sideRoom in sideRooms)
        {
            foreach (GameObject node in sideRoom.nodes)
            {
                NodeController nc = node.GetComponent<NodeController>();

                // Check rooms for potential wall positions
                foreach (GameObject room in sideRoom.nodes)
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
    }

    private void GenerateSideRooms()
    {
        // find potential side rooms
        foreach (GameObject worldNode in worldNodes)
        {
            NodeController nc = worldNode.GetComponent<NodeController>();

            float randomChestChance = Random.Range(0.0f, 100.0f);

            // if theres no room to the east
            if (!nc.east)
            {
                SideRoomInfo sideRoomInfo = new SideRoomInfo(worldNode, true, randomChestChance);

                potentialSideRooms.Add(sideRoomInfo);
            }
            // if theres no room to the west
            if (!nc.west)
            {
                SideRoomInfo sideRoomInfo = new SideRoomInfo(worldNode, false, randomChestChance);

                potentialSideRooms.Add(sideRoomInfo);                
            }
        }

        // check max is not too high
        if (maxSideRooms > potentialSideRooms.Count)
        {
            maxSideRooms = potentialSideRooms.Count;
        }

        // set side room separation
        int sideRoomSpacing = potentialSideRooms.Count / maxSideRooms;

        // find next floor room to place a side room in
        for (int index = 0; index < maxSideRooms; index++)
        {
            int rng = Random.Range((index * sideRoomSpacing), ((index + 1) * sideRoomSpacing));

            // add room to side rooms list
            sideRooms.Add(potentialSideRooms[rng]);            
        }

        // instantiate side room node
        for (int i = 0; i < sideRooms.Count; i++)
        {
            GameObject teleporterRoomNode = Instantiate(worldNodePrefab, sideRooms[i].worldNode.transform);

            sideRooms[i].nodes.Add(teleporterRoomNode);

            GameObject teleporterRoom = Instantiate(teleporterRoomPrefab, teleporterRoomNode.transform);

            float probability = Random.Range(0.0f, 100.0f);

            teleporterRoom.GetComponentInChildren<Portal>().SetMaterialColour(probability);

            NodeController nc = teleporterRoomNode.GetComponent<NodeController>();

            if (sideRooms[i].toEast)
            {
                nc.west = true;

                teleporterRoomNode.transform.position = sideRooms[i].worldNode.transform.position + new Vector3(10, 0, 0);

                sideRooms[i].worldNode.GetComponent<NodeController>().east = true;
            }
            else
            {
                nc.east = true;

                teleporterRoomNode.transform.position = sideRooms[i].worldNode.transform.position + new Vector3(-10, 0, 0);

                sideRooms[i].worldNode.GetComponent<NodeController>().west = true;
            }

            GameObject teleporterDestinationRoomNode = Instantiate(worldNodePrefab, teleporterRoomNode.transform);
            GameObject teleporterDestinationRoom = Instantiate(teleporterRoomPrefab, teleporterDestinationRoomNode.transform);
                       
            teleporterDestinationRoomNode.transform.position = new Vector3(0, -(((generatedFloor - 1) * 100) + (i * 10)) - 10, 0);

            sideRooms[i].nodes.Add(teleporterDestinationRoomNode);

            // init teleporters
            Teleporter mainPortal = teleporterRoom.GetComponent<Teleporter>();
            Teleporter sidePortal = teleporterDestinationRoom.GetComponent<Teleporter>();

            sidePortal.sideRoom = true;
            mainPortal.destination = sidePortal.portal.position;
            sidePortal.destination = mainPortal.portal.position;

            profile.AddSideRoomToTotal();
        }
    }

    private void FillSideRooms()
    {
        foreach (SideRoomInfo sideRoom in sideRooms)
        {
            NodeInfo newPos = new NodeInfo(sideRoom.nodes[1].transform.position, sideRoom.nodes[1].transform.position);

            for (int i = 0; i < sideRoomLength; i++)
            {
                GameObject nextRoomNode = Instantiate(worldNodePrefab, sideRoom.nodes[1].transform);

                sideRoom.nodes.Add(nextRoomNode);

                newPos = FindNextPosition(newPos);
                               
                nextRoomNode.GetComponent<NodeController>().previous = newPos.previousPosition;

                sideRoom.nodes[sideRoom.nodes.Count - 1].GetComponent<NodeController>().next = newPos.nextPosition;

                nextRoomNode.transform.position = newPos.nextPosition;

                Instantiate(roomPrefab, nextRoomNode.transform);

                if (i == sideRoomLength - 1)
                {
                    float chestRNG = Random.Range(0.0f, 100.0f);

                    if (chestRNG >= sideRoom.chestProbability)
                    {
                        Instantiate(chestPrefab, nextRoomNode.transform);
                    }
                }
            }
        }
    }

    private void GenerateWalls()
    {
        // world node rooms
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

        // side rooms
        foreach (SideRoomInfo sideRoom in sideRooms)
        {
            foreach (GameObject node in sideRoom.nodes)
            {
                NodeController nc = node.GetComponent<NodeController>();

                //instantiate walls
                if (!nc.north)
                {
                    Instantiate(wallPrefab, nc.northWall);
                }
                if (!nc.east)
                {
                    Instantiate(wallPrefab, nc.eastWall);
                }
                if (!nc.south)
                {
                    Instantiate(wallPrefab, nc.southWall);
                }
                if (!nc.west)
                {
                    Instantiate(wallPrefab, nc.westWall);
                }
            }
        }
    }
}