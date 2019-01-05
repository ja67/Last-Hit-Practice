using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infantry : PhysicsObject
{
    private int teamId;
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

    protected override void Awake()
    {
        base.Awake();
        teamId = 0;
    }
    // Use this for initialization
    new void Start()
    {
        speed = 2f;
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




}
