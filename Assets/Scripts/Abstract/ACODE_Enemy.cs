using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    public abstract class ACODE_Enemy : ACODE_Characters
    {
        public Animator enemyAnimator;

        protected Rigidbody2D RB;

        public bool IsGrounded;

        #region PATROL SETTINGS
        [Header("Enemy Patrol Settings")]
        public Transform[] patrolPoints;
        public float waitTime;
        public bool isAtPatrolPoint;
        #endregion

        [Header("Alternative Patrol")]
        public Transform leftLimit;
        public Transform rightLimit;

        #region CHASE SETTINGS
        [Header("Enemy Chase Settings")]
        public Transform target;
        public float outOfRadiusDelay;
        public float outOfRadiusTimer;
        #endregion

        #region SPEED VARIABLES
        [Header("Enemy Speed")]
        public float patrolSpeed;
        public float chaseSpeed;
        #endregion

        #region ENEMY STANCE BOOLEANS
        [Header("Enemy Stances")]
        public bool isPatrolling = true;
        public bool isChasing = false;
        public bool isAttacking = false;
        public bool isCooling = false;
        public bool isInRange = false;
        #endregion

        #region CHECK FACE DIRECTION
        protected float checkFaceTime = 0.2f;
        #endregion

        #region ATTACK SETTINGS
        public float _attackCooldown;
        public float _attackCooldownTimer;

        #region RAYCAST PUBLIC VARIABLES
        [Header("Raycast Settings")]
        public Transform raycastTransform;
        public LayerMask raycastMask;
        public float raycastLength;

        [Space(5)]
        #endregion

        #region RAYCAST PROTECTED VARIABLES
        public RaycastHit2D _hit;
        public Transform _raycastTarget;
        #endregion

        #region DISTANCE PUBLIC VARIABLES
        [Header("Attack Settings")]
        public float attackDistance;
        #endregion

        #region DISTANCE PROTECTED VARIABLES
        public float _distanceToPlayer;
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
            yield return new WaitForSeconds(waitTime);
            if (_currentPatrolPointIndex + 1 < patrolPoints.Length)
                _currentPatrolPointIndex++;
            else
            {
                _currentPatrolPointIndex = 0;
            }

            isAtPatrolPoint = false;
        }

        protected void RaycastDebugger()
        {
            if (_distanceToPlayer > attackDistance)
            {
                if (_IsFacingRight)
                    Debug.DrawRay(raycastTransform.position, transform.right * raycastLength, Color.red);
                else
                    Debug.DrawRay(raycastTransform.position, -transform.right * raycastLength, Color.red);
            }
            else if (_distanceToPlayer <= attackDistance)
            {
                if (_IsFacingRight)
                    Debug.DrawRay(raycastTransform.position, transform.right * raycastLength, Color.green);
                else
                    Debug.DrawRay(raycastTransform.position, -transform.right * raycastLength, Color.green);
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

            if (_distanceToPlayer > attackDistance)
            {
                PerformChase();
                StopAttack();
            }
            else if (_distanceToPlayer <= attackDistance && !isCooling)
            {
                isChasing = false;
                isPatrolling = false;
                PerformAttack();
            }
        }

        protected void PerformAttack()
        {
            _attackCooldownTimer = _attackCooldown;
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
                    followTarget(target.position.x, chaseSpeed);
                }
            }

        }

        protected void PerformPatrol()
        {
            enemyAnimator.SetBool("CanWalk", true);
            if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttackingAnim"))
            {
                if (isPatrolling && !isAttacking && !isChasing)
                {
                    if (transform.position.x != patrolPoints[_currentPatrolPointIndex].position.x)
                    {
                        followTarget(patrolPoints[_currentPatrolPointIndex].position.x, patrolSpeed);
                    }
                    else
                    {
                        if (isAtPatrolPoint == false)
                        {
                            isAtPatrolPoint = true;
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
                _attackCooldownTimer = _attackCooldown;
            }
            else
            {
                Debug.DrawRay(new Vector2(raycastTransform.position.x, raycastTransform.position.y + 2f), Vector2.right * raycastLength, Color.yellow);
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
            transform.position = Vector2.MoveTowards(transform.position, Target, Speed * Time.deltaTime);
        }

    }

}

