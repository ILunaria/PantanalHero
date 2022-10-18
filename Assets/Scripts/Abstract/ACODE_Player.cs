using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CHARACTERS
{
	public class ACODE_Player : ACODE_Characters
	{
		public PlayerData Data;

		public PlayerInputActions _playerInputActions;

		#region COMPONENTS
		public Rigidbody2D RB { get; protected set; }
		public Animator ANIM { get; protected set; }
		//Script to handle all player animations, all references can be safely removed if you're importing into your own project.
		#endregion

		#region STATE PARAMETERS
		//Variables control the various actions the player can perform at any time.
		//These are fields which can are public allowing for other sctipts to read them
		//but can only be privately written to.
		public bool IsJumping { get; protected set; }
		public bool IsDashing { get; protected set; }

		public bool IsBlocking;

		public bool IsAttacking;

		public bool AttackInput = false;
		public bool IsWallJumping { get; protected set; }

		//Timers
		public float LastOnGroundTime { get; protected set; }
		public float LastOnWallTime { get; protected set; }
		public float LastOnWallRightTime { get; protected set; }
		public float LastOnWallLeftTime { get; protected set; }
		public float LastPressedJumpTime { get; protected set; }
		public float LastPressedDashTime { get; protected set; }

		public float LastPressedBlockTime;

		//Jump
		public bool _isJumpCut { get; protected set; }
		public bool _isJumpFalling { get; protected set; }

		//Wall Jump
		protected float _wallJumpStartTime;
		protected int _lastWallJumpDir;

		//Dash
		protected int _dashesLeft;
		protected bool _dashRefilling;
		protected Vector2 _lastDashDir;
		protected bool _isDashAttacking;

		//Block

		protected int _blocksLeft;
		protected bool _blockRefilling;

		// Attack
		public Transform attackPoint;
		public float attackRange;
		public LayerMask enemyLayers;
		public float _attackRate = 2f;
		protected float nextAttackTime = 0f;

		#endregion

		#region INPUT PARAMETERS
		protected Vector2 _moveInput;

		#endregion

		#region CHECK PARAMETERS
		//Set all of these up in the inspector
		[Header("Checks")]
		[SerializeField] protected Transform _groundCheckPoint;
		//Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
		[SerializeField] protected Vector2 _groundCheckSize;
		[Space(5)]
		[SerializeField] protected Transform _frontWallCheckPoint;
		[SerializeField] protected Transform _backWallCheckPoint;
		[SerializeField] protected Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
		#endregion

		#region LAYERS & TAGS
		[Header("Layers & Tags")]
		[SerializeField] protected LayerMask _groundLayer;
		#endregion

		#region INPUT SYSTEM
		public void JumpAction_Performed(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				OnJumpInput();
			}
			else if (context.canceled)
			{
				OnJumpUpInput();
			}
		}

		public void DashAction_Performed(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				OnDashInput();
			}
		}

		public void AttackAction_Performed(InputAction.CallbackContext context)
        {
			if(context.started)
            {
				AttackInput = true;
            }
        }

		public void BlockAction_Performed(InputAction.CallbackContext context)
        {
			if(context.started)
            {
				OnBlockInput();
			}
        }

		#endregion

		#region INPUT CALLBACKS
		//Methods which handle input detected in Update()
		// Jump Buffering
		public void OnJumpInput()
		{
			LastPressedJumpTime = Data.jumpInputBufferTime;
		}

		public void OnBlockInput()
        {
			LastPressedBlockTime = Data.blockInputBufferTime;
        }

		public void OnJumpUpInput()
		{
			if (CanJumpCut())
				_isJumpCut = true;
		}

		public void OnDashInput()
		{
			LastPressedDashTime = Data.dashInputBufferTime;
		}
		#endregion

		#region GENERAL METHODS
		public void SetGravityScale(float scale)
		{
			RB.gravityScale = scale;
		}

		protected void Sleep(float duration)
		{
			//Method used so we don't need to call StartCoroutine everywhere
			//nameof() notation means we don't need to input a string directly.
			//Removes chance of spelling mistakes and will improve error messages if any
			StartCoroutine(nameof(PerformSleep), duration);
		}

		private IEnumerator PerformSleep(float duration)
		{
			Time.timeScale = 0;
			yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
			Time.timeScale = 1;
		}
		#endregion

		#region RUN METHODS
		protected void Run(float lerpAmount)
		{
			//Calculate the direction we want to move in and our desired velocity
			float targetSpeed = _moveInput.x * Data.runMaxSpeed;
			//We can reduce our control using Lerp() this smooths changes to our direction and speed
			targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount); // a + ( b - a ) * Clamp01(t)

			#region Calculate AccelRate
			float accelRate;

			//Gets an acceleration value based on if we are accelerating (includes turning) 
			//or trying to decelerate (stop). As well as applying a multiplier if we're in air.
			if (LastOnGroundTime > 0)
				accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
			else
				accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
			#endregion

			#region Add Bonus Jump Apex Acceleration
			//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
			if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
			{
				accelRate *= Data.jumpHangAccelerationMult; // Increases Acceleration Rate Whilst on Jump Apex
				targetSpeed *= Data.jumpHangMaxSpeedMult; // Increases Max Speed Whilst on Jump Apex
			}
			#endregion

			#region Conserve Momentum
			//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
			if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
			{
				//Prevent any deceleration from happening, or in other words conserve our current momentum
				//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
				accelRate = 0;
			}
			#endregion

			//Calculate difference between current velocity and desired velocity
			float speedDif = targetSpeed - RB.velocity.x;
			//Calculate force along x-axis to apply to thr player

			float movement = speedDif * accelRate;

			ANIM.SetFloat("Speed", Mathf.Abs(RB.velocity.x));
			//Convert this to a vector and apply to rigidbody
			RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
		}
		#endregion

		#region JUMP METHODS
		protected void Jump()
		{
			//Ensures we can't call Jump multiple times from one press
			LastPressedJumpTime = 0;
			LastOnGroundTime = 0;

			#region Perform Jump
			//We increase the force applied if we are falling
			//This means we'll always feel like we jump the same amount 
			float force = Data.jumpForce;
			if (RB.velocity.y < 0)
				force -= RB.velocity.y;

			RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
			#endregion
		}

		protected void WallJump(int dir)
		{
			//Ensures we can't call Wall Jump multiple times from one press
			LastPressedJumpTime = 0;
			LastOnGroundTime = 0;
			LastOnWallRightTime = 0;
			LastOnWallLeftTime = 0;

			#region Perform Wall Jump
			Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
			force.x *= dir; //apply force in opposite direction of wall

			if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
				force.x -= RB.velocity.x;

			if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
				force.y -= RB.velocity.y;

			RB.AddForce(force, ForceMode2D.Impulse);
			#endregion
		}
		#endregion

		#region DASH METHODS
		//Dash Coroutine
		protected IEnumerator StartDash(Vector2 dir) // For Multi Direction Dash Use This: private IEnumerator StartDash(Vector2 dir)
		{
			
			dir = dir.normalized * Vector2.right; // Horizontal Dash Only
												  //Overall this method of dashing aims to mimic Celeste, if you're looking for
												  // a more physics-based approach try a method similar to that used in the jump
			LastOnGroundTime = 0;
			LastPressedDashTime = 0;

			float startTime = Time.time;

			_dashesLeft--;
			_isDashAttacking = true;
			

			SetGravityScale(0);

			ANIM.SetBool("IsDashing", _isDashAttacking);

			//We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
			while (Time.time - startTime <= Data.dashDragTime)
			{
				RB.velocity = dir.normalized * Data.dashSpeed;
				//Pauses the loop until the next frame, creating something of a Update loop. 
				//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
				yield return null;
			}

			startTime = Time.time;

			_isDashAttacking = false;

			ANIM.SetBool("IsDashing", _isDashAttacking);

			//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
			SetGravityScale(Data.gravityScale);
			RB.velocity = Data.dashEndSpeed * dir.normalized;

			while (Time.time - startTime <= Data.dashEndTime)
			{
				yield return null;
			}

			//Dash over
			IsDashing = false;
		}

		protected IEnumerator StartBlock()
        {
			LastPressedBlockTime = 0;
			float startTime = Time.time;
			_blocksLeft--;

			//blockHitBox.gameObject.SetActive(true);

			while (Time.time - startTime <= Data.blockTimeAmount)
			{
				yield return null;
			}

			//blockHitBox.gameObject.SetActive(false);
			IsBlocking = false;
		}

		protected void StartAttack()
        {
			Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

			foreach(Collider2D enemy in hitEnemies)
            {
				Debug.Log("We Hit" + enemy.name);
            }
        }

		//Short period before the player is able to dash again
		private IEnumerator RefillDash() // int amount
		{
			//SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
			_dashRefilling = true;
			yield return new WaitForSeconds(Data.dashRefillTime);
			_dashRefilling = false;
			_dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
		}
		#endregion

		private IEnumerator RefillBlock() // int amount
		{
			
			_blockRefilling = true;
			yield return new WaitForSeconds(Data.blockRefillTime);
			_blockRefilling = false;
			_blocksLeft = Mathf.Min(Data.blockAmount, _blocksLeft + 1);
		}

		#region CHECK METHODS
		// LastOnGrounTime refers to the player staying on the ground or if he's fallen from the platform but still is able to jump for some time.
		protected bool CanJump()
		{
			return LastOnGroundTime > 0 && !IsJumping;
		}

		private bool CanJumpCut()
		{
			return IsJumping && RB.velocity.y > 0;
		}
		protected bool CanDash()
		{
			if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
			{
				StartCoroutine(nameof(RefillDash)); // StartCoroutine(nameof(RefillDash), 1);
			}

			return _dashesLeft > 0;
		}

		protected bool CanAttack()
        {
			return !IsDashing && !IsWallJumping;
        }

		protected bool CanBlock()
		{
			if (!IsBlocking && _blocksLeft < Data.blockAmount && !_blockRefilling && !IsDashing)
            {
				StartCoroutine(nameof(RefillBlock));
			}
			return _blocksLeft > 0;
		}
		protected bool CanWallJump()
		{
			return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
				 (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
		}

		private bool CanWallJumpCut()
		{
			return IsWallJumping && RB.velocity.y > 0;
		}
		#endregion
	}
}