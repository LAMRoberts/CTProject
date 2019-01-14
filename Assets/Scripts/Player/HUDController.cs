using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public Canvas nextFloor;
    public Canvas previousFloor;

    private Canvas next;
    private Canvas previous;

    private void Start()
    {
        next = Instantiate(nextFloor, transform);
        previous = Instantiate(previousFloor, transform);

        next.enabled = false;
        previous.enabled = false;
    }

    public void NextFloor()
    {
        next.enabled = !next.enabled;
    }

    public void PreviousFloor()
    {
        previous.enabled = !previous.enabled;
    }
}
