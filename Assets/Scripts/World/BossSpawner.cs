using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab;
    public Transform startPosition;

	// Use this for initialization
	void Start ()
    {
        GameObject boss = Instantiate(bossPrefab, transform);

        boss.transform.position = startPosition.position;
	}
}
