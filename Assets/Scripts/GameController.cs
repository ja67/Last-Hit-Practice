using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public Camera cam;
    public GameObject infrantry;

    public float timeLeft;
    public Text timerText;
    public int lastHitCount;
    public Text LastHitText;

    public Player player;
    public List<Infantry> friendlyInfantryList;
    public List<Infantry> enemyInfantryList;

    private const int numOfSpawnCreep = 3;


    // Use this for initialization
    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        friendlyInfantryList = new List<Infantry>();
        enemyInfantryList = new List<Infantry>();

        StartCoroutine(Spawn());
        UpdateText();
    }

    // Update is called once physics timestamp
    private void FixedUpdate()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            timeLeft = 0;
        }
        UpdateText();
    }

    private void UpdateText()
    {
        timerText.text = "Time Left:\n" + Mathf.RoundToInt(timeLeft);
    }


    Infantry SpawnSingleCreep(Vector3 spawnPosition, int teamId)
    {
        GameObject newInfantryObj = Instantiate(infrantry, spawnPosition, Quaternion.identity);
        Infantry newInfantry = newInfantryObj.GetComponent<Infantry>() as Infantry;
        newInfantry.TeamId = teamId;
        newInfantry.gameController = this;
        return newInfantry;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                player.targetEnemy = hit.transform.gameObject;
            }
            else
            {
                player.targetEnemy = null;
                player.targetPosition = mousePos;
            }
        }
    }

    IEnumerator Spawn()
    {
        for (int i = 0; i < numOfSpawnCreep; i++)
        {
            //Spawn Friendly Creep
            Vector2 pos = new Vector2(Infantry.xLimit, Random.Range(-Infantry.yLimit, Infantry.yLimit));
            friendlyInfantryList.Add(SpawnSingleCreep(pos, 1));
            //Spawn Enemy Creep
            pos = new Vector2(-Infantry.xLimit, Random.Range(-Infantry.yLimit, Infantry.yLimit));
            friendlyInfantryList.Add(SpawnSingleCreep(pos, 2));
        }
        yield return new WaitForSeconds(30);
    }

}
