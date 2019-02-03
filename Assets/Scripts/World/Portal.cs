using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{	
    public void SetMaterialColour(float probability)
    {
        if (probability == 0.0f)
        {
            GetComponent<Renderer>().material.color = new Color(0.0f, 0.75f, 0.75f, 0.25f);
        }
        else
        {
            float interp = probability / 100.0f;

            GetComponent<Renderer>().material.color = new Color(1.0f - interp, interp, 0.0f, 0.25f);
        }
    }
}
