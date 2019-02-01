using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyUI
{
    public RectTransform healthBar;
    public GameObject enemy;

    public EnemyUI(RectTransform hp, GameObject guy)
    {
        healthBar = hp;
        enemy = guy;
    }
}

public class HUDController : MonoBehaviour
{
    public Canvas activatePrefab;
    private Canvas activate;

    public RectTransform healthBarPrefab;

    public List<EnemyUI> enemyUIs;

    private GameObject player;
    private Camera cam;

    private void Start()
    {
        enemyUIs = new List<EnemyUI>();

        player = GameObject.FindGameObjectWithTag("Player");

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        activate = Instantiate(activatePrefab, transform);

        activate.enabled = false;
    }

    private void Update()
    {
        foreach (EnemyUI enemyUI in enemyUIs)
        {
            RaycastHit hit;
            if (Physics.Raycast(enemyUI.enemy.transform.position, (player.transform.position - enemyUI.enemy.transform.position), out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    Debug.DrawRay(enemyUI.enemy.transform.position, (player.transform.position - enemyUI.enemy.transform.position) * hit.distance, Color.green);

                    if (!enemyUI.healthBar.gameObject.activeSelf)
                    {
                        enemyUI.healthBar.gameObject.SetActive(true);
                    }

                    Vector3 pos = cam.WorldToScreenPoint(enemyUI.enemy.GetComponent<Enemy>().healthBarPoint.position);
                    enemyUI.healthBar.position = pos;
                }
                else
                {
                    Debug.DrawRay(enemyUI.enemy.transform.position, (player.transform.position - enemyUI.enemy.transform.position) * hit.distance, Color.red);

                    if (enemyUI.healthBar.gameObject.activeSelf)
                    {
                        enemyUI.healthBar.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                Debug.DrawRay(enemyUI.enemy.transform.position, (player.transform.position - enemyUI.enemy.transform.position) * hit.distance, Color.blue);

            }
        }
    }

    public void Activate(bool on)
    {
        activate.enabled = on;
    }

    public void AddEnemyHealthBar(GameObject enemy)
    {
        // instantiate 
        RectTransform health = Instantiate(healthBarPrefab, transform);

        health.GetComponent<UIHealthBarController>().SetActor(enemy);

        health.localScale = new Vector3(health.localScale.x * 0.1f, health.localScale.y * 0.1f, health.localScale.z * 0.1f);

        // add object to list
        enemyUIs.Add(new EnemyUI(health, enemy));
    }
}
