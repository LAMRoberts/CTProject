using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinParticlesController : MonoBehaviour
{
    private bool go;
    private ParticleSystem ps;
    private bool done = false;
	
	void Update ()
    {
        if (go)
        {
            if (!done)
            {
                ps.transform.position = transform.position;

                done = true;
            }

            ps.transform.position = Vector3.MoveTowards(ps.transform.position, transform.position, 1.0f);
        }
	}

    public void Go()
    {
        go = true;
    }

    public void SetParticleSystem(ParticleSystem p)
    {
        ps = p;
    }
}
