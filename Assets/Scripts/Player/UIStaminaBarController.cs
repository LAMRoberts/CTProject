using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStaminaBarController : MonoBehaviour
{
    public GameObject player;
    private PlayerMovement pm;

    [SerializeField]
    private float stamina = 100.0f;

    public RectTransform staminaBar;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pm = player.GetComponent<PlayerMovement>();
    }

    void Update ()
    {
        if (stamina != pm.stamina)
        {
            stamina = pm.stamina;

            staminaBar.transform.localScale = new Vector3(stamina / 100, staminaBar.transform.localScale.y, staminaBar.transform.localScale.z);
        }
	}
}
