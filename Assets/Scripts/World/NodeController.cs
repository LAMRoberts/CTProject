using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public Vector3 previous;
    public Vector3 next;

    public bool north;
    public bool east;
    public bool south;
    public bool west;

    public Transform northWall;
    public Transform eastWall;
    public Transform southWall;
    public Transform westWall;
}
