using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrail : MonoBehaviour
{
    public Transform sword;

    ParticleSystem ps;

    void Start ()
    {
        ps = GetComponent<ParticleSystem>();

        ps.Stop();
    }

    void Update()
    {

    }

    public void Trail(bool value)
    {
        if (value)
        {
            ps.Play();
        }
        else
        {
            ps.Stop();
        }
    }
}
