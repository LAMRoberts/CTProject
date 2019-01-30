using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthBarController : MonoBehaviour
{
    public GameObject player;
    private Actor actor;

    [SerializeField]
    private float health = 100.0f;

    public RectTransform healthBar;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        actor = player.GetComponent<Actor>();
    }

    void Update ()
    {
        if (health != actor.GetHealth())
        {
            health = actor.GetHealth();

            healthBar.transform.localScale = new Vector3(health / 100, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        }
	}
}
