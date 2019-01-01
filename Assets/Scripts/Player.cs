using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PhysicsObject
{
    public override void Start()
    {
        speed = 2f;
        maxHealthPoint = 10000000f;
        healthPoint = maxHealthPoint;
        targetEnemy = null;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetEnemy)
        {
            targetPosition = targetEnemy.GetComponent<Rigidbody2D>().position;
        }
    }
    protected override void Attack()
    {
        float afterAttackHp = targetEnemy.GetComponent<PhysicsObject>().healthPoint -= 10;
        if (afterAttackHp <= 0f)
        {
            targetEnemy = null;
            myAnimator.SetBool("isAttacking", false);
        }
    }
}
