using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public Canvas activatePrefab;
    private Canvas activate;

    private void Start()
    {
        activate = Instantiate(activatePrefab, transform);

        activate.enabled = false;
    }

    public void Activate(bool on)
    {
        activate.enabled = on;
    }
}
