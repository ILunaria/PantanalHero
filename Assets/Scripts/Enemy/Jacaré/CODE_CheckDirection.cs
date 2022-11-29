using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODE_CheckDirection : StateMachineBehaviour
{
    private Transform enemyPos;
    private Transform playerPos;
    public bool _IsFacingRight;

    private Vector3 originalScale;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyPos = GameObject.Find("Jacaré_Blindado").GetComponent<Transform>();
        originalScale = enemyPos.localScale;

        playerPos = GameObject.Find("Player").GetComponent<Transform>();


        _IsFacingRight = originalScale.x > 0 ? false : true;
        CheckDirectionToFace(playerPos.position.x + enemyPos.position.x > 0);
        
    }

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != _IsFacingRight)
            Turn();
    }

    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = enemyPos.localScale;
        scale.x *= -1;
        enemyPos.localScale = scale;

        _IsFacingRight = !_IsFacingRight; // false
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
