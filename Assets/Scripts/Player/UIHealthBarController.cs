using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthBarController : MonoBehaviour
{
    public GameObject player;
    private PlayerController pc;

    [SerializeField]
    private float health = 100.0f;

    public RectTransform healthBar;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
    }

    void Update ()
    {
        if (health != pc.health)
        {
            health = pc.health;

            healthBar.transform.localScale = new Vector3(health / 100, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        }
	}
}
