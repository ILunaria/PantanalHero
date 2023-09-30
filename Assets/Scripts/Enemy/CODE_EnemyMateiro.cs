using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CHARACTERS
{
    public class CODE_EnemyMateiro : ACODE_Enemy
    {
        public bool insideOfLimits;

        void Start()
        {
            this._checkSurroundings = transform.GetChild(0).GetComponent<CODE_PlayerTriggerZone>();
            this.faceTimer = checkFaceTime;

            this._attackCooldownTimer = EnemyData.attackCooldown;

            RB = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {

            Debug.DrawRay(frontWallRayCastTransform.position, -transform.up * rayToGroundLength, Color.blue);
         
            if(_IsFacingRight)
            {
                Debug.DrawRay(this.raycastTransform.position, transform.right * 1f, Color.blue);
            }
            else
            {
                Debug.DrawRay(this.raycastTransform.position, -transform.right * 1f, Color.blue);
            }

            insideOfLimits = checkWallLimits();

            #region TO DO

            if ((isPatrolling || isChasing) && !isAtPatrolPoint && checkGroundLimits())
            {
                enemyAnimator.SetBool("CanWalk", true);
            }
            else
            {
                enemyAnimator.SetBool("CanWalk", false);
            }

            if(isPatrolling && (!checkGroundLimits() || checkWallLimits()))
            {
               _currentPatrolPointIndex = 0;
            }

            #endregion

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

            if (isGrounded)
                CheckAroundEnemy();
        }

        private void CheckAroundEnemy()
        {
            if (_checkSurroundings.isInsideOfRange && checkGroundLimits())
            {
                isPatrolling = false;
                outOfRadiusTimer = EnemyData.outOfRadiusDelay;

                if (_IsFacingRight)
                    this._hit = Physics2D.Raycast(this.raycastTransform.position, transform.right, this.rayToPlayerLength, this.playerMask);
                else
                    this._hit = Physics2D.Raycast(this.raycastTransform.position, -transform.right, this.rayToPlayerLength, this.playerMask);

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
                else if (checkGroundLimits())
                {
                    isPatrolling = false;
                    isChasing = true;
                    isAttacking = false;
                    isCooling = false;
                    PerformChase();
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(frontWallCheckPoint.position, wallCheckSize);
            Gizmos.DrawWireCube(backWallCheckPoint.position, wallCheckSize);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.name.StartsWith("Ground"))
                isGrounded = true;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.name.StartsWith("Ground"))
                isGrounded = false;
        }
    }
}
