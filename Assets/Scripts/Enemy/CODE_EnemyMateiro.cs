using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CHARACTERS
{
    public class CODE_EnemyMateiro : ACODE_Enemy
    {
        void Start()
        {
            this._checkSurroundings = transform.GetChild(0).GetComponent<CODE_PlayerTriggerZone>();
            this.faceTimer = checkFaceTime;

            this._attackCooldownTimer = this._attackCooldown;

            RB = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            #region TIMERS
            this.faceTimer -= Time.deltaTime;
            this.outOfRadiusTimer -= Time.deltaTime;
            #endregion

            if (faceTimer < 0)
            {
                faceTimer = checkFaceTime;
                if (isPatrolling)
                {
                    CheckPos(patrolPoints[_currentPatrolPointIndex].position.x);
                }
                else if (isChasing)
                {
                    CheckPos(target.position.x);
                }
            }

            CheckAroundEnemy();
        }

        private void CheckAroundEnemy()
        {
            if (_checkSurroundings.isInsideOfRange)
            {
                isPatrolling = false;
                outOfRadiusTimer = outOfRadiusDelay;

                if(_IsFacingRight)
                    this._hit = Physics2D.Raycast(this.raycastTransform.position, transform.right, this.raycastLength, this.raycastMask);
                else
                    this._hit = Physics2D.Raycast(this.raycastTransform.position, -transform.right, this.raycastLength, this.raycastMask);

                this._raycastTarget = _checkSurroundings.collisionTarget.transform;
                RaycastDebugger();

               
                if (this._hit.collider != null)
                {
                    EnemyAttackLogic();
                }
            }
            else
            {
                if (outOfRadiusTimer < 0)
                {
                    enemyAnimator.SetBool("Attack", false);
                    isPatrolling = true;
                    isChasing = false;
                    isAttacking = false;
                    isCooling = false;
                    PerformPatrol();
                }
                else
                {
                    isPatrolling = false;
                    isChasing = true;
                    isAttacking = false;
                    isCooling = false;
                    PerformChase();
                }
            }
        }
    }
}
