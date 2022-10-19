using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CHARACTERS
{
	public class CODE_PlayerMovement : ACODE_Player
	{
		private void Awake()
		{
			RB = GetComponent<Rigidbody2D>();
			ANIM = GetComponent<Animator>();

			#region INPUT SYSTEM

			_playerInputActions = new PlayerInputActions();
			_playerInputActions.Player.Enable();

			_playerInputActions.Player.Jump.started += JumpAction_Performed;
			_playerInputActions.Player.Jump.canceled += JumpAction_Performed;

			_playerInputActions.Player.Dash.started += DashAction_Performed;

			_playerInputActions.Player.Attack.started += AttackAction_Performed;

			_playerInputActions.Player.Block.started += BlockAction_Performed;

			#endregion
		}

		private void Start()
		{
			SetGravityScale(Data.gravityScale);
			this._IsFacingRight = true;
		}

		private void Update()
		{
			#region TIMERS
			// Jump Timers
			LastOnGroundTime -= Time.deltaTime;
			LastPressedJumpTime -= Time.deltaTime;

			// Wall Jump Timers
			LastOnWallTime -= Time.deltaTime;
			LastOnWallRightTime -= Time.deltaTime;
			LastOnWallLeftTime -= Time.deltaTime;

			// Dash Timers
			LastPressedDashTime -= Time.deltaTime;

			// Block Timers
			LastPressedBlockTime -= Time.deltaTime;

			#endregion

			#region INPUT HANDLER

			_moveInput = _playerInputActions.Player.Movement.ReadValue<Vector2>();

			if (_moveInput.x != 0)
				CheckDirectionToFace(_moveInput.x > 0);

			#endregion

			#region COLLISION CHECKS
			if (!IsJumping)
			{
				//Ground Check
				if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping) //checks if set box overlaps with ground
				{
					LastOnGroundTime = Data.coyoteTime; //if so sets the lastGrounded to coyoteTime
					ANIM.SetBool("isGrounded", true);
				}

				//Left Wall Check
				if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && this._IsFacingRight)
					   || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !this._IsFacingRight)) && !IsWallJumping)
					LastOnWallRightTime = Data.coyoteTime;

				//Right Wall Check
				if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !this._IsFacingRight)
					|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && this._IsFacingRight)) && !IsWallJumping)
					LastOnWallLeftTime = Data.coyoteTime;

				//Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
				LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
			}
            #endregion 

            #region JUMP CHECKS
            // Check if the player is falling
            if (IsJumping && RB.velocity.y < 0)
			{
				IsJumping = false;
				ANIM.SetTrigger("Fall");
				if (!IsWallJumping)
					_isJumpFalling = true;
			}

			if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
			{
				IsWallJumping = false;
			}

			// Check if the player is on ground
			if (LastOnGroundTime > 0 && !IsJumping) // if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
			{
				_isJumpCut = false;
				_isJumpFalling = false;
			}

			if (!IsDashing)
			{
				//JUMP
				if (CanJump() && LastPressedJumpTime > 0) // return LastOnGroundTime > 0 && !IsJumping;
				{
					IsJumping = true;
					IsWallJumping = false;
					_isJumpCut = false;
					_isJumpFalling = false;
					Jump();

				}
				//WALL JUMP
				else if (CanWallJump() && LastPressedJumpTime > 0)
				{
					IsWallJumping = true;
					IsJumping = false;
					_isJumpCut = false;
					_isJumpFalling = false;

					_wallJumpStartTime = Time.time;
					_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

					WallJump(_lastWallJumpDir);
				}
			}
			#endregion

			#region DASH CHECKS
			if (CanDash() && LastPressedDashTime > 0)
			{
				//Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
				Sleep(Data.dashSleepTime);

				//If no direction is pressed, dash forward
				if (_moveInput.x != 0f)
					_lastDashDir = _moveInput;
				else
					_lastDashDir = this._IsFacingRight ? Vector2.right : Vector2.left;



				IsDashing = true;
				
				IsJumping = false;
				IsWallJumping = false;
				_isJumpCut = false;

				StartCoroutine(nameof(StartDash), _lastDashDir);
			}
            #endregion

            #region ATTACK CHECK

            if (Time.time >= nextAttackTime)
            {
                if (AttackInput && CanAttack())
                {
					AttackInput = false;
                    IsAttacking = true;
                    StartAttack();
                    nextAttackTime = Time.time + 1f / _attackRate;
                }
				else
                {
					IsAttacking = false;
				}
            }
            #endregion

            #region BLOCK CHECK

            if (CanBlock() && LastPressedBlockTime > 0)
            {
				IsBlocking = true;
                ANIM.SetTrigger("Defense");
                StartCoroutine(nameof(StartBlock));
			}

			#endregion

			#region GRAVITY
			if (!_isDashAttacking)
			{
				//Higher gravity if we've released the jump input or are falling
				if (RB.velocity.y < 0 && _moveInput.y < 0) // else if
				{
					//Much higher gravity if holding down
					SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
					//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
					RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
				}
				else if (_isJumpCut)
				{
					//Higher gravity if jump button released, so you can perform higher jumps or lower jumps
					SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
					RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
				}
				else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold) // Increases Air Time
				{
					SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
				}
				else if (RB.velocity.y < 0)
				{
					//Higher gravity if falling
					SetGravityScale(Data.gravityScale * Data.fallGravityMult);
					//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
					RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));


				}
				else
				{
					//Default gravity if standing on a platform or moving upwards
					SetGravityScale(Data.gravityScale);
				}
			}
			else
			{
				//No gravity when dashing (returns to normal once initial dashAttack phase over)
				SetGravityScale(0);
			}
			#endregion
		}

		private void FixedUpdate()
		{
			//Handle Run
			if (!IsDashing)
			{
				if (IsWallJumping)
					Run(Data.wallJumpRunLerp);
				else
					Run(1);
			}
			else if (_isDashAttacking)
			{
				Run(Data.dashEndRunLerp); // Reduces Player Target Speed Whilst Dashing
			}
		}

		#region EDITOR METHODS
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
			Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);

			if(IsAttacking)
            {
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(attackPoint.position, attackRange);
			}
			else
            {
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(attackPoint.position, attackRange);
			}
				

		}
        #endregion

        private void OnTriggerEnter2D(Collider2D trigger)
        {
			if(trigger.gameObject.name == "Hit_Box")
            {
				if(IsBlocking)
                {
					GameObject enemy = trigger.transform.parent.gameObject;
					CODE_EnemyMateiro enemyCode = enemy.GetComponent<CODE_EnemyMateiro>();

					//enemyCode.isCooling = true;
					//enemyCode.isChasing = false;

					Vector2 knockbackDir = new Vector2(transform.position.x - enemy.transform.position.x, 0f);
					RB.velocity = new Vector2(knockbackDir.x, 0f) * 4f;
					enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(-knockbackDir.x, 0f) * 4f;
					Debug.Log("Attack Blocked");
                }
				else if(!IsBlocking)
                {
					Debug.Log("You are Dead");
                }
            }
        }
    }
}
