using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODE_Salto : StateMachineBehaviour
{
    public GameObject splash;
    public bool hasSpawned;

    private Transform enemyPos;

    private Vector3 scale;

    private Vector3 saveScale;

    public int isFacingRight;

    public float random;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        random = Random.Range(0, 2);
        enemyPos = GameObject.Find("Jacaré_Blindado").GetComponent<Transform>();


        saveScale = enemyPos.localScale;
        switch (random)
        {
            case 0:
                scale = enemyPos.localScale;
                scale.x *= -1;
                enemyPos.localScale = scale;
                isFacingRight = -1;
                break;
            case 1:
                enemyPos.localScale = saveScale;
                isFacingRight = 1;
                break;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyPos.position = new Vector2(-5f * isFacingRight, -15f);
        //if(animator.transform.position.y < - 2f && !hasSpawned)
        //{
        //    Instantiate(splash, new Vector2(-5f, -2f), Quaternion.identity);
        //    hasSpawned = true;
        //}

        //animator.transform.position = Vector2.MoveTowards(animator.transform.position, new Vector2(-15f, -20f), 40f * Time.deltaTime);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyPos.localScale = saveScale;

        bool willStun = Random.Range(0, 5) < 3f ? true : false;

        animator.SetBool("Stunned", willStun);
        animator.transform.position = new Vector2(animator.transform.position.x, -25f);
    }

}
