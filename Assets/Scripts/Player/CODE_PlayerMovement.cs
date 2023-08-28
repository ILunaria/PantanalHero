using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace CHARACTERS
{
	public class CODE_PlayerMovement : ACODE_Player
	{
		private Light2D lightScene;
		bool isDying;

        private void Awake()
		{
			RB = GetComponent<Rigidbody2D>();
			ANIM = GetComponent<Animator>();
			lightScene = FindObjectOfType<Light2D>().GetComponent<Light2D>();
			Time.timeScale = 1f;
            isDying = false;
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

			// Attack Timers
			LastPressedAttackTime -= Time.deltaTime;

			// Block Timers
			LastPressedBlockTime -= Time.deltaTime;

            //Death Timer

            if (isDying)
            {
				Time.timeScale -= Time.deltaTime*4;
                lightScene.intensity -= Time.deltaTime*4;
            }

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
					LastOnGroundTime = PlayerData.coyoteTime; //if so sets the lastGrounded to coyoteTime
					IsWallSliding = false;
					ANIM.SetBool("IsWallSliding", false);
					ANIM.SetBool("isGrounded", true);
				}

				//Left Wall Check
				if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _wallLayer) && this._IsFacingRight)
					   || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _wallLayer) && !this._IsFacingRight)) && !IsWallJumping)
				{
					LastOnWallRightTime = PlayerData.coyoteTime;
				}


				//Right Wall Check
				if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _wallLayer) && !this._IsFacingRight)
					|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _wallLayer) && this._IsFacingRight)) && !IsWallJumping)
				{
					LastOnWallLeftTime = PlayerData.coyoteTime;
				}


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
				ANIM.SetBool("IsFalling", true);
				if (!IsWallJumping)
				{
					_isJumpFalling = true;
				}
			}

			if (IsWallJumping && Time.time - _wallJumpStartTime > PlayerData.wallJumpTime)
			{
				IsWallJumping = false;
			}

			// Check if the player is on ground
			if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping) // if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
			{
				ANIM.SetBool("isGrounded", true);
				ANIM.SetBool("IsFalling", false);
				IsGrounded = true;
				_isJumpCut = false;
				_isJumpFalling = false;
			}

			if (!IsDashing && !IsBlocking)
			{
				//JUMP
				if (CanJump() && LastPressedJumpTime > 0) // return LastOnGroundTime > 0 && !IsJumping;
				{
					IsGrounded = false;
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

					IsGrounded = false;

					ANIM.SetTrigger("Jump");
					ANIM.SetBool("IsFalling", false);

					_wallJumpStartTime = Time.time;
					_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

					WallJump(_lastWallJumpDir);
				}
			}
			#endregion

			#region WALL SLIDING CHECKS

			if (LastOnWallTime == PlayerData.coyoteTime && RB.velocity.y < 0)
			{
				IsWallSliding = true;
				ANIM.SetBool("IsWallSliding", true);
			}
			else
			{
				IsWallSliding = false;
				ANIM.SetBool("IsWallSliding", false);
			}

			#endregion

			#region DASH CHECKS
			if (CanDash() && LastPressedDashTime > 0 && !IsBlocking)
			{
				//Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
				Sleep(PlayerData.dashSleepTime);

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

			if (CanAttack() && LastPressedAttackTime > 0)
			{
				IsAttacking = true;
				StartCoroutine(nameof(StartAttack));
			}
			#endregion

			#region BLOCK CHECK

			if (CanBlock() && LastPressedBlockTime > 0 && RB.velocity.y == 0)
			{
				IsBlocking = true;
				RB.velocity = new Vector2(0f, RB.velocity.y);
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
					ANIM.SetTrigger("Fall");
					ANIM.SetBool("IsFalling", true);
					//Much higher gravity if holding down
					SetGravityScale(PlayerData.gravityScale * PlayerData.fastFallGravityMult);
					//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
					RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -PlayerData.maxFastFallSpeed));
				}
				else if (_isJumpCut)
				{
					//Higher gravity if jump button released, so you can perform higher jumps or lower jumps
					SetGravityScale(PlayerData.gravityScale * PlayerData.jumpCutGravityMult);
					RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -PlayerData.maxFallSpeed));
				}
				else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < PlayerData.jumpHangTimeThreshold) // Increases Air Time
				{
					SetGravityScale(PlayerData.gravityScale * PlayerData.jumpHangGravityMult);
				}
				else if (RB.velocity.y < 0)
				{
					ANIM.SetBool("IsFalling", true);
					//Higher gravity if falling
					SetGravityScale(PlayerData.gravityScale * PlayerData.fallGravityMult);
					//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
					RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -PlayerData.maxFallSpeed));
				}
				else
				{
					ANIM.SetBool("IsFalling", false);
					//Default gravity if standing on a platform or moving upwards
					SetGravityScale(PlayerData.gravityScale);
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
			if (!IsBlocking)
			{
				if (!IsDashing)
				{
					if (IsWallJumping)
						Run(PlayerData.wallJumpRunLerp);
					else
						Run(1);
				}
				else if (_isDashAttacking)
				{
					Run(PlayerData.dashEndRunLerp); // Reduces Player Target Speed Whilst Dashing
				}
			}
		}

		#region EDITOR METHODS
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
			Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);

			if (IsAttacking)
			{
				Gizmos.color = Color.green;
				//Gizmos.DrawWireSphere(attackPoint.position, attackRange);

				if(_IsFacingRight)
					Gizmos.DrawWireCube(new Vector2(((attackPoint.position.x + (attackHitBox.x / 2)) - 0.8f), attackPoint.position.y), attackHitBox);
				else
					Gizmos.DrawWireCube(new Vector2(((attackPoint.position.x - (attackHitBox.x / 2)) + 0.8f), attackPoint.position.y), attackHitBox);
			}
			else
			{
				Gizmos.color = Color.red;
				//Gizmos.DrawWireSphere(attackPoint.position, attackRange);
				if (_IsFacingRight)
					Gizmos.DrawWireCube(new Vector2(((attackPoint.position.x + (attackHitBox.x / 2)) - 0.8f), attackPoint.position.y), attackHitBox);
				else
					Gizmos.DrawWireCube(new Vector2(((attackPoint.position.x - (attackHitBox.x / 2)) + 0.8f), attackPoint.position.y), attackHitBox);
			} 
		}
		#endregion

		private void OnTriggerEnter2D(Collider2D trigger)
		{
			if (trigger.gameObject.name == "Hit_Box")
			{
				if (IsBlocking)
				{
					GameObject enemy = trigger.transform.parent.gameObject;
					blockVFX.Play();

					while(enemy.tag != "Enemy")
                    {
						enemy = enemy.transform.parent.gameObject;
                    }

					CODE_EnemyMateiro enemyCode = enemy.GetComponent<CODE_EnemyMateiro>();

					Vector2 knockbackDir = new Vector2(transform.position.x - enemy.transform.position.x, 0f);
					RB.velocity = new Vector2(knockbackDir.x, PlayerData.selfYAxisKnockback == true ? 1f : 0f) * PlayerData.selfKnockbackAmount;
					enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(-knockbackDir.x, PlayerData.selfYAxisKnockback == true ? 1f : 0f) * PlayerData.enemyKnockbackAmount;
					Debug.Log("Attack Blocked");

				}
				else if (!IsBlocking)
                {
                    isDying = true;

                    StartCoroutine(GameOver(isDying));

					//GameOver � aqui
				}
			}
		}
		IEnumerator GameOver(bool value)
		{
			yield return new WaitForSecondsRealtime(0.3f);
			value = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
	}
}
