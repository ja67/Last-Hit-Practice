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
    }

    // Update is called once per frame
    void Update()
    {
        if (targetEnemy)
        {
            targetPosition = targetEnemy.GetComponent<Rigidbody2D>().position;
        }
    }
    protected override void AttackAction()
    {
        Infantry targetCreep = targetEnemy.GetComponent<PhysicsObject>() as Infantry;
        float afterAttackHp = targetCreep.healthPoint -= 10;
        if (afterAttackHp <= 0f)
        {
            targetEnemy = null;
        }
        else
        {
            // TODO: Create a defaultdict?
            if (targetCreep.AggroMap.ContainsKey(gameObject))
            {
                targetCreep.AggroMap[gameObject] += 1;
            }
            else
            {
                targetCreep.AggroMap[gameObject] = 1;

            }
        }
    }
}
