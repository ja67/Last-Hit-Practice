using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    [HideInInspector] public Camera cam;
    public SoundManager SoundManager;
    public GameObject infrantry;

    [HideInInspector] public float timeLeft;
    [HideInInspector] public Text timerText;
    [HideInInspector] public int lastHitCount;
    [HideInInspector] public Text LastHitText;

    public Player player;
    [HideInInspector] public List<GameObject> friendlyObjectList;
    [HideInInspector] public List<GameObject> enemyObjectList;

    private const int numOfSpawnCreep = 3;


    // Use this for initialization

    private void Awake()
    {
        player.gameController = this; 
    }
    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        friendlyObjectList = new List<GameObject>();
        enemyObjectList = new List<GameObject>();

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
        foreach (GameObject friend in friendlyObjectList)
        {
            Dictionary<GameObject, int> friendAggroMap = friend.GetComponent<Infantry>().AggroMap;
            foreach (GameObject enemy in enemyObjectList)
            {
                Dictionary<GameObject, int> enemyAggroMap = enemy.GetComponent<Infantry>().AggroMap;
                if (!friendAggroMap.ContainsKey(enemy))
                {
                    int initAggro = Mathf.RoundToInt(Mathf.Abs((enemy.gameObject.transform.position - friend.gameObject.transform.position).magnitude));
                    friendAggroMap[enemy] = initAggro;
                    enemyAggroMap[friend] = initAggro;
                }
            }
        }
        foreach (GameObject friend in friendlyObjectList)
        {
            Infantry friendInfantry = friend.GetComponent<Infantry>();
            friendInfantry.targetEnemy = friendInfantry.AggroMap.FirstOrDefault(x => x.Value == friendInfantry.AggroMap.Values.Max()).Key;
        }
        foreach (GameObject enemy in enemyObjectList)
        {
            Infantry enemyInfantry = enemy.GetComponent<Infantry>();
            enemyInfantry.targetEnemy = enemyInfantry.AggroMap.FirstOrDefault(x => x.Value == enemyInfantry.AggroMap.Values.Max()).Key;
        }
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
            friendlyObjectList.Add(SpawnSingleCreep(pos, 1).gameObject);
            //Spawn Enemy Creep
            pos = new Vector2(-Infantry.xLimit, Random.Range(-Infantry.yLimit, Infantry.yLimit));
            enemyObjectList.Add(SpawnSingleCreep(pos, 2).gameObject);
        }
        yield return new WaitForSeconds(30);
    }

}
