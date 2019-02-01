using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthBarController : MonoBehaviour
{
    public GameObject character;
    private Actor actorController;

    [SerializeField]
    private float health = 100.0f;

    public RectTransform healthBar;

    void Start()
    {
        if (character == null)
        {
            character = GameObject.FindGameObjectWithTag("Player");
            actorController = character.GetComponent<Actor>();
        }
    }

    void Update ()
    {
        if (health != actorController.GetHealth())
        {
            health = actorController.GetHealth();

            healthBar.transform.localScale = new Vector3(health / 100, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        }
	}

    public void SetActor(GameObject a)
    {
        character = a;
        actorController = character.GetComponent<Actor>();

        Debug.Log("Health Bar Assigned");
    }
}
