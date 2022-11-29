using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODE_IntroStage : StateMachineBehaviour
{


    private int _random;

    private float time = 1f;

    public float speed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        Debug.Log("Intro Stage Has Started");

        _random = Random.Range(0, 3);

        switch (_random)
        {
            case 0:
                animator.SetTrigger("Rabada");
                break;
            case 1:
                animator.SetTrigger("Mordida");
                break;
            case 2:
                animator.SetTrigger("Salto");
                break;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (time < 0)
            animator.transform.position = Vector2.MoveTowards(animator.transform.position, new Vector2(animator.transform.position.x, -25f), speed * Time.deltaTime);
        else
            time -= Time.deltaTime;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.position = new Vector2(0f, -25f);
    }

}
