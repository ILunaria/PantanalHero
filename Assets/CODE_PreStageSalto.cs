using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODE_PreStageSalto : StateMachineBehaviour
{
    private Transform enemyPos;

    private Vector3 saveScale;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyPos = GameObject.Find("Jacaré_Blindado").GetComponent<Transform>();
        saveScale = enemyPos.localScale;

        enemyPos.localScale = Vector3.one * 0.25f;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyPos.position = new Vector2(0f, -20f);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyPos.localScale = saveScale;
    }
}
