using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Infantry : PhysicsObject
{
    private int teamId;
    [HideInInspector]
    public int TeamId
    {
        get
        {
            return teamId;
        }
        set
        {
            Color color = Color.white;
            if (value == 1)
            {
                color = Color.blue;
            }
            else if (value == 2)
            {
                color = Color.red;
            }
            color.a = 0.5f;
            myRenderer.color = color;
            teamId = value;
        }
    }
    protected Canvas nestedCanvas;
    protected Slider hpBar;

    protected override void Awake()
    {
        base.Awake();

        teamId = 0;
        nestedCanvas = GetComponentInChildren<Canvas>();
        nestedCanvas.renderMode = RenderMode.WorldSpace;
        nestedCanvas.sortingLayerName = "Infantry";
        nestedCanvas.worldCamera = myCam;
        nestedCanvas.transform.position = transform.position;

        hpBar = GetComponentInChildren<Slider>();
        hpBar.transform.localPosition = new Vector2(0f, objectHeight * 0.8f);
    }
    // Use this for initialization
    new void Start()
    {
        moveSpeed = 2f;
        maxHealthPoint = 100f;
        healthPoint = maxHealthPoint;
        base.Start();
        currentPosition = new Vector2(-1f, -1f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    protected override IEnumerator Die()
    {
        if (teamId == 1)
        {
            gameController.friendlyObjectList.Remove(gameObject);
        }
        else if (teamId == 2)
        {
            gameController.enemyObjectList.Remove(gameObject);
        }
        foreach (GameObject opposite in AggroMap.Keys)
        {
            Dictionary<GameObject, int> oppoAggroMap = opposite.GetComponent<PhysicsObject>().AggroMap;
            if (oppoAggroMap.ContainsKey(gameObject))
            {
                oppoAggroMap.Remove(gameObject);
            }
        }
        yield return base.Die();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        hpBar.value = healthPoint / maxHealthPoint;
        //Hp bar gradually changes from green to red when wounded
        hpBar.fillRect.gameObject.GetComponent<Image>().color = new  Color(1- hpBar.value, hpBar.value, 0f, 1f);
    }




}
