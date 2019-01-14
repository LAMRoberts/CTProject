using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInfo : MonoBehaviour
{
    public GameObject levelPrefab;

    public List<GameObject> levels;

    private int lowestFloor = 1;
    public int playerFloor = 1;
    public int startElevatorFloor = 1;
    public int endElevatorFloor = 1;

	void Start ()
    {
        levels = new List<GameObject>();

        GameObject level = Instantiate(levelPrefab);

        levels.Add(level);

        Debug.Log(levels.Count);
    }

    private void Update()
    {
        if (lowestFloor > levels.Count)
        {
            GameObject level = Instantiate(levelPrefab);

            level.transform.position = new Vector3(0, 0 - (lowestFloor * 100), 0);

            levels.Add(level);

            Debug.Log(levels.Count);
        }
    }

    public void SetLowestFloor(int floorNumber)
    {
        if (lowestFloor < floorNumber)
        {
            lowestFloor = floorNumber;
        }
    }

    public int GetLowestFloor()
    {
        return lowestFloor;
    }
}
