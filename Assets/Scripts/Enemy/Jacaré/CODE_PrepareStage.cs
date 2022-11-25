using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class CODE_PrepareStage : StateMachineBehaviour
{
    public GameObject attackFeedbackEffect;

    private Transform playerPos;

    public Vector2 playerOldPos;

    private Transform enemyPos;

    public float trackTime;
    private float trackTimeCounter;
    public  bool canAttack;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        enemyPos = GameObject.Find("Jacaré_Blindado").GetComponent<Transform>();

        trackTimeCounter = trackTime;

        canAttack = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (trackTimeCounter < 1 && !canAttack)
        {
            canAttack = true;
            playerOldPos = new Vector2(playerPos.position.x, playerPos.position.y);
            Destroy(Instantiate(attackFeedbackEffect, new Vector2(playerOldPos.x, -2f), Quaternion.identity), 1f);
            trackTimeCounter = trackTime;
            enemyPos.position = new Vector2(playerOldPos.x, -37f);
            animator.SetTrigger("CanAttack");
        }
        else
        {
            trackTimeCounter -= Time.deltaTime;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
}
