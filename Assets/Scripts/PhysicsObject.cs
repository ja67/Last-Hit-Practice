using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{

    protected float speed;
    public float healthPoint;
    protected float maxHealthPoint;

    public GameController gameController;
    protected Animator myAnimator;
    protected Rigidbody2D myRigidbody2D;
    protected SpriteRenderer myRenderer;
    public Camera myCam;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[5];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(5);

    protected Vector2 currentPosition;
    public Vector2 targetPosition;

    private const float minMove = 0.01f;
    private const float minDistance = 0.5f;
    private const float shellRadius = 0.01f;

    public GameObject targetEnemy; 
    public Dictionary<GameObject, int> AggroMap { get; set; } //TODO: Find a more efficient way to get enemy with largest aggro

    private static float objectWidth;
    private static float objectHeight;

    public static float xLimit;
    public static float yLimit;

    protected virtual void Awake()
    {
        if (!myCam)
        {
            myCam = Camera.main;
        }
        AggroMap = new Dictionary<GameObject, int>();
        Vector2 upperCorner = new Vector2(Screen.width, Screen.height);
        Vector2 targetUpperCorner = myCam.ScreenToWorldPoint(upperCorner);
        myRenderer = GetComponent<SpriteRenderer>();
        objectWidth = myRenderer.bounds.extents.x;
        objectHeight = myRenderer.bounds.extents.y;
        xLimit = targetUpperCorner.x - objectWidth;
        yLimit = targetUpperCorner.y - objectHeight;

    }
    public virtual void Start()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void FixedUpdate()
    {
        currentPosition = myRigidbody2D.position;

        Vector2 velocity = (targetPosition - currentPosition).normalized * speed;
        if (CalculateHitBuffer(velocity))
        {
            RaycastHit2D hit = hitBuffer[0];
            Vector2 hitNormal = hit.normal;
            // If the object is about to hit another, change the direction of the velocity to the tagential direction 
            velocity = GetVelocityAfterCollision(velocity, hit, hitNormal);
        }
        myRigidbody2D.velocity = velocity;
        if(myRigidbody2D.position.x >= xLimit)
        {
            myRigidbody2D.position = new Vector2(xLimit, myRigidbody2D.position.y);
        }
        if(myRigidbody2D.position.y >= yLimit)
        {
            myRigidbody2D.position = new Vector2(myRigidbody2D.position.x, yLimit);
        }
        if(myRigidbody2D.position.x <= -xLimit)
        {
            myRigidbody2D.position = new Vector2(-xLimit, myRigidbody2D.position.y);
        }
        if(myRigidbody2D.position.y <= -yLimit)
        {
            myRigidbody2D.position = new Vector2(myRigidbody2D.position.x, -yLimit);
        }
        UpdateAnimation(velocity);
    }

    protected void UpdateAnimation(Vector2 velocity)
    {
        if (healthPoint <= 0f)
        {
            StartCoroutine(Die());
        }
        else if ((targetPosition - currentPosition).magnitude > minDistance)
        {
            myAnimator.SetBool("isRunning", true);
            float horizontalMove = velocity.x * Time.deltaTime;
            if ((horizontalMove > minMove && myRenderer.flipX) || (horizontalMove < -minMove && !myRenderer.flipX))
            {
                myRenderer.flipX = !myRenderer.flipX;
            }
        }
        else
        {
            myRigidbody2D.velocity = Vector2.zero;
            myAnimator.SetBool("isRunning", false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == targetEnemy)
        {
            myAnimator.SetBool("isRunning", false);
            myAnimator.SetBool("isAttacking", true);
            Vector2 enemyDirection = (targetEnemy.GetComponent<Rigidbody2D>().position - currentPosition).normalized;
            myRenderer.flipX = enemyDirection.x < 0;
            Attack();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == targetEnemy)
        {
            myAnimator.SetBool("isAttacking", false);
        }
    }

    private bool CalculateHitBuffer(Vector2 velocity)
    {
        hitBuffer = new RaycastHit2D[5];
        int count = myRigidbody2D.Cast(velocity, contactFilter, hitBuffer, speed * Time.deltaTime + shellRadius);
        return count > 0;

    }

    private Vector2 GetVelocityAfterCollision(Vector2 velocity, RaycastHit2D hit, Vector2 hitNormal)
    {
        float projection = Vector2.Dot(myRigidbody2D.velocity, hitNormal);
        if (projection < 0)
        {
            Vector2 tangentialSpeed = Vector2.Perpendicular(hitNormal);
            Vector2 hitPostion = hit.point;
            Vector2 hitRigid2DPos = hit.rigidbody.position;
            Vector2 velocityDirection;
            float objectShortestRouteDirection = Vector2.Dot((hitPostion - hitRigid2DPos).normalized, tangentialSpeed);
            System.Console.WriteLine(objectShortestRouteDirection);
            if (Mathf.Abs(objectShortestRouteDirection) > 0.05f)
            {
                velocityDirection = objectShortestRouteDirection > 0 ? tangentialSpeed : -tangentialSpeed;
            }
            else
            {
                float targetDirectShortestRouteDirection = Vector2.Dot(targetPosition - hitPostion, tangentialSpeed);
                velocityDirection = targetDirectShortestRouteDirection > 0 ? tangentialSpeed : -tangentialSpeed;

            }
            velocity = speed * velocityDirection;
        }

        return velocity;
    }
    protected virtual IEnumerator Die()
    {
        myAnimator.SetBool("isDead", true);
        myAnimator.SetBool("isAttacking", false);
        myAnimator.SetBool("isRunning", false);
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }


    protected virtual void Attack()
    {

    }


}
