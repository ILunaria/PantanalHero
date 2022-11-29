using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODE_PreStageRabada : StateMachineBehaviour
{
    private Transform enemyPos;

    public bool _IsFacingRight;
    private Transform playerPos;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        enemyPos = GameObject.Find("Jacaré_Blindado").GetComponent<Transform>();

        if (playerPos.position.x + enemyPos.position.x > 0)
        {
            _IsFacingRight = false;
        }
        else
        {
            _IsFacingRight = true;
        }

        if (_IsFacingRight)
            enemyPos.position = new Vector2(playerPos.position.x - 5f, -26f);
        else
            enemyPos.position = new Vector2(playerPos.position.x + 5f, -26f);

    }
}
