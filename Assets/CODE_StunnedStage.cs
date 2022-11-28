using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODE_StunnedStage : StateMachineBehaviour
{
    private Transform enemyPos;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyPos = GameObject.Find("Jacaré_Blindado").GetComponent<Transform>();

        enemyPos.position = new Vector2(15f, -8f);

        enemyPos.GetChild(2).gameObject.SetActive(true);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyPos.position = Vector2.MoveTowards(enemyPos.position, new Vector2(15f, -2f), 10f * Time.deltaTime);
    }

}
