using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infantry : PhysicsObject {
    private int teamId;
    public int TeamId
    {
        get
        {
            return this.teamId;
        }
        set
        {
            Color color = Color.white;
            if (value == 1)
            {
                color = Color.red;
            }
            else if(value == 2){
                color = Color.blue;
            }
            color.a = 0.5f;
            myRenderer.color = color;
            this.teamId = value;
        }
    }

	// Use this for initialization
	new void Start () {
        speed = 2f;
        maxHealthPoint = 100f;
        healthPoint = maxHealthPoint;
        base.Start();
        currentPosition = new Vector2(-1f, -1f);
        teamId = 0;
	}
	
	// Update is called once per frame
	void Update () {
	}

    


}
