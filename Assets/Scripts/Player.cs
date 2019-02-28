using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PhysicsObject
{
    protected override void Awake()
    {
        base.Awake();
        moveSpeed = 2f;
        maxHealthPoint = 10000000f;
        healthPoint = maxHealthPoint;
        targetEnemy = null;
        attackFrequency = 0.5f;
        attackPoint = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetEnemy)
        {
            targetPosition = targetEnemy.GetComponent<Rigidbody2D>().position;
        }
    }
}
