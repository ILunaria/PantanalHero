using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    public abstract class ACODE_Enemy : ACODE_Characters
    {
        public CODE_EnemyData EnemyData;

        public Animator enemyAnimator;

        protected Rigidbody2D RB;

        #region PATROL SETTINGS
        [Header("Enemy Patrol Settings")]
        public Transform[] patrolPoints;
        public bool isAtPatrolPoint;
        #endregion

        #region CHASE SETTINGS
        [Header("Enemy Chase Settings")]
        public Transform target;
        protected float outOfRadiusTimer;
        #endregion

        #region ENEMY STANCE BOOLEANS
        [Header("Enemy Stances")]
        public bool isPatrolling = true;
        public bool isChasing = false;
        public bool isAttacking = false;
        public bool isCooling = false;
        public bool isInRange = false;
        public bool isGrounded;
        #endregion

        #region CHECK FACE DIRECTION
        protected float checkFaceTime = 0.2f;
        #endregion

        #region CHECK WALL
        public Transform frontWallCheckPoint;
        public Transform backWallCheckPoint;
        public Vector2 wallCheckSize;

        public Transform frontWallRayCastTransform;
        public Transform backWallRayCastTransform;
        public LayerMask groundMask;
        public LayerMask wallMask;
        public float rayToGroundLength;
        #endregion

        #region ATTACK SETTINGS
        protected float _attackCooldownTimer;

        #region RAYCAST PUBLIC VARIABLES
        [Header("Raycast Settings")]
        public Transform raycastTransform;
        public LayerMask playerMask;
        public float rayToPlayerLength;

        [Space(5)]
        #endregion

        #region RAYCAST PROTECTED VARIABLES
        public RaycastHit2D _hit;
        public Transform _raycastTarget;
        #endregion

        #region DISTANCE PROTECTED VARIABLES
        protected float _distanceToPlayer;
        #endregion
        #endregion

        protected float faceTimer;
        protected int _currentPatrolPointIndex;
        protected CODE_PlayerTriggerZone _checkSurroundings;

        protected void CheckPos(float targetXPos)
        {
            if (!isAtPatrolPoint)
                CheckDirectionToFace(transform.position.x < targetXPos);
        }
        protected IEnumerator Wait()
        {
            yield return new WaitForSeconds(EnemyData.patrolWaitTime);

            if (_currentPatrolPointIndex + 1 < patrolPoints.Length)
                _currentPatrolPointIndex++;
            else
            {
                _currentPatrolPointIndex = 0;
            };
            isAtPatrolPoint = false;
            enemyAnimator.SetBool("CanWalk", true);
        }

        protected void RaycastDebugger()
        {
            if (_distanceToPlayer > EnemyData.attackDistance)
            {
                if (_IsFacingRight)
                    Debug.DrawRay(raycastTransform.position, transform.right * rayToPlayerLength, Color.red);
                else
                    Debug.DrawRay(raycastTransform.position, -transform.right * rayToPlayerLength, Color.red);
            }
            else if (_distanceToPlayer <= EnemyData.attackDistance)
            {
                if (_IsFacingRight)
                    Debug.DrawRay(raycastTransform.position, transform.right * rayToPlayerLength, Color.green);
                else
                    Debug.DrawRay(raycastTransform.position, -transform.right * rayToPlayerLength, Color.green);
            }
        }

        protected void EnemyAttackLogic()
        {
            _distanceToPlayer = Vector2.Distance(transform.position, _raycastTarget.position);

            if (isCooling)
            {
                enemyAnimator.SetBool("Attack", false);
                AttackCooldown();
            }

            if (_distanceToPlayer > EnemyData.attackDistance)
            {
                PerformChase();
                StopAttack();
            }
            else if (_distanceToPlayer <= EnemyData.attackDistance && !isCooling)
            {
                isChasing = false;
                isPatrolling = false;
                PerformAttack();
            }
        }

        protected void PerformAttack()
        {
            _attackCooldownTimer = EnemyData.attackCooldown;
            isAttacking = true;

            enemyAnimator.SetBool("Attack", true);
        }

        public void StopAttack()
        {
            //isCooling = false;
            isAttacking = false;
            enemyAnimator.SetBool("Attack", false);
            enemyAnimator.SetBool("CanWalk", true);
        }

        protected void PerformChase()
        {
            if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttackingAnim"))
            {
                isChasing = true;
                if (isChasing && !isPatrolling && !isAttacking)
                {
                    followTarget(target.position.x, EnemyData.chaseSpeed);
                }
            }

        }

        protected void PerformPatrol()
        {
            if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttackingAnim"))
            {
                if (isPatrolling && !isAttacking && !isChasing)
                {
                    if (transform.position.x != patrolPoints[_currentPatrolPointIndex].position.x)
                    {
                        followTarget(patrolPoints[_currentPatrolPointIndex].position.x, EnemyData.patrolSpeed);
                    }
                    else
                    {
                        if (isAtPatrolPoint == false)
                        {
                            isAtPatrolPoint = true;
                            enemyAnimator.Play("Idle");
                            enemyAnimator.SetBool("CanWalk", false);
                            StartCoroutine(Wait());
                        }
                    }
                }
            }

        }

        protected void AttackCooldown()
        {
            _attackCooldownTimer -= Time.deltaTime;

            if (_attackCooldownTimer <= 0 && isCooling)
            {
                isCooling = false;
                _attackCooldownTimer = EnemyData.attackCooldown;
            }
            else
            {
                Debug.DrawRay(new Vector2(raycastTransform.position.x, raycastTransform.position.y + 2f), Vector2.right * rayToPlayerLength, Color.yellow);
            }
        }

        protected void TriggerCooldown()
        {
            Debug.Log("COOLDOWN");
            isCooling = true;
        }

        private void followTarget(float TargetX, float Speed)
        {
            Vector2 Target = new Vector2(TargetX, transform.position.y);
            if(checkWallLimits() == false)
            {
                transform.position = Vector2.MoveTowards(transform.position, Target, Speed * Time.deltaTime);
            }
        }

        protected bool checkGroundLimits()
        {
            RaycastHit2D Hit = Physics2D.Raycast(frontWallRayCastTransform.position, -transform.up, rayToGroundLength, groundMask);

            return Hit.collider != null ? true : false;
        }

        protected bool checkWallLimits()
        {
            RaycastHit2D Hit;
            
            if(_IsFacingRight)
                Hit = Physics2D.Raycast(this.raycastTransform.position, transform.right, 1f, this.wallMask);
            else
                Hit = Physics2D.Raycast(this.raycastTransform.position, -transform.right, 1f, this.wallMask);

            return Hit.collider != null ? true : false;
        }

    }

}

