using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBehaviour : MonoBehaviour
{
    public SpriteRenderer fovCircle;

    [Header("Enemy Patrol Settings")]
    public Transform[] patrolPoints;
    public float waitTime;
    public bool isAtPatrolPoint;

    [Header ("Enemy Speed")]
    public float patrolSpeed;
    public float chaseSpeed;

    [Header("Follow AI Settings")]
    public Transform target;
    public float fieldOfViewRadius;
    public float outOfRadiusDelay;

    public float outOfRadiusTimer;

    private int currentPointIndex;
    private bool IsFacingRight = true;


    public bool doPatrol = true;
    public bool doChase = false;

    public float checkFaceTime = 0.1f;
    private float faceTime;
    


    private void Start()
    {
        faceTime = checkFaceTime;
    }

    private void CheckPos(float targetXPos)
    {
        if (!isAtPatrolPoint)
            CheckDirectionToFace(transform.position.x < targetXPos);
    }

    private void Update()
    {
        faceTime -= Time.deltaTime;
        outOfRadiusTimer -= Time.deltaTime;

        if(faceTime < 0)
        {
            faceTime = checkFaceTime;
            if(doPatrol)
            {
                fovCircle.color = Color.blue;
     
                CheckPos(patrolPoints[currentPointIndex].position.x);
            }
            else if(doChase)
            {
                fovCircle.color = Color.red;
                CheckPos(target.position.x);
            }

        }

        if (Vector2.Distance(transform.position, target.position) <= fieldOfViewRadius)
        {
            doPatrol = false;
            doChase = true;
            outOfRadiusTimer = outOfRadiusDelay;
        }
        else
        {
            if(outOfRadiusTimer < 0)
            {
                doPatrol = true;
                doChase = false;
            }
        }

        if (doPatrol)
        {
            if (transform.position.x != patrolPoints[currentPointIndex].position.x)
            {

                Vector2 targetX = new Vector2(patrolPoints[currentPointIndex].position.x, transform.position.y);
                followTarget(targetX, patrolSpeed);
            }
            else
            {
                if (isAtPatrolPoint == false)
                {
                    isAtPatrolPoint = true;
                    StartCoroutine(Wait());
                }

            }
        }

        if(doChase)
        {
            Vector2 targetX = new Vector2(target.position.x, transform.position.y);
            followTarget(targetX, chaseSpeed);
        }
        
    }

    private void followTarget(Vector2 t, float followSpeed)
    {
        transform.position = Vector2.MoveTowards(transform.position, t, followSpeed * Time.deltaTime);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        if(currentPointIndex + 1 < patrolPoints.Length)
            currentPointIndex++;
        else
        {
            currentPointIndex = 0;
        }

        isAtPatrolPoint = false;
    }

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight; // false
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fieldOfViewRadius);
    }
}
