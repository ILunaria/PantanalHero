using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    #region GRAVITY
	[HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
	[HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
												 //Also the value the player's rigidbody2D.gravityScale is set to.
	[Header("Gravity")]
	[Tooltip("Multiplier to the player's gravityScale when falling.")]
	public float fallGravityMult;
	[Tooltip ("Maximum fall speed of the player when falling.")]
	public float maxFallSpeed;
	[Space(5)]
	[Tooltip("Larger multiplier to the player's gravityScale when they are falling and pressing downwards.")]
	public float fastFallGravityMult;
	[Tooltip("Maximum fall speed of the player when performing a faster fall.")]
	public float maxFastFallSpeed;

	[Space(20)]
	#endregion

	#region ATTACK
	[Header("Attack")]
	[Tooltip("Time between next attack.")]
	public float attackInputBufferTime;

	[Space(20)]
	#endregion

	#region BLOCK
	[Header("Block")]
	[Tooltip("Time between next block.")]
	public float blockInputBufferTime;
	public float blockTimeAmount;
	public int blockAmount;
	public float blockRefillTime;

	[Space(20)]
	#endregion

	#region RUN
	[Header("Run")]
	[Tooltip("Target speed we want the player to reach.")]
	public float runMaxSpeed;
	[Tooltip("The speed at which the player accelerates to max speed.")]
	public float runAcceleration;
	[HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
	public float runDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
	[HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
	[Space(5)]
	[Tooltip("Acceleration multiplier applied to acceleration rate when airborne.")]
	[Range(0f, 1)] public float accelInAir;
	[Tooltip("Desacceleration multiplier applied to acceleration rate when airborne.")]
	[Range(0f, 1)] public float deccelInAir;
	[Space(5)]
	[Tooltip ("Conserve momentum if the player is moving with the desired direction but at a greater speed than its maxSpeed.")]
	public bool doConserveMomentum = true;

	[Space(20)]
    #endregion

    #region JUMP
    [Header("Jump")]
	[Tooltip("Height of the player's jump.")]
	public float jumpHeight;
	[Tooltip("Time necessary to reach the jump apex.")]
	public float jumpTimeToApex;
	[HideInInspector] public float jumpForce; //The actual force applied (upwards) to the player when they jump.
	#endregion

	#region WALL JUMP
	[Header("Wall Jump")]
	[Tooltip("Force applied on the X and Y directions while performing the wall jump.")]
	public Vector2 wallJumpForce;
	[Space(5)]
	[Tooltip("Reduces the effect of player's movement while wall jumping.")]
	[Range(0f, 1f)] public float wallJumpRunLerp;
	[Tooltip("Time after wall jumping which the player's movement is slowed for.")]
	[Range(0f, 1.5f)] public float wallJumpTime;
	[Tooltip("Player will rotate to face wall jumping direction.")]
	public bool doTurnOnWallJump;
	#endregion

	#region JUMP EFFECTS
	[Header("Jump Effects")]
	[Tooltip("Multiplier to increase gravity if the player releases the jump button while still jumping.")]
	public float jumpCutGravityMult;
	[Tooltip("Reduces gravity while close to the apex of the jump.")]
	[Range(0f, 1f)] public float jumpHangGravityMult;
	[Tooltip("Extra jumpHang.")]
	public float jumpHangTimeThreshold;
	[Space(0.5f)]
	[Tooltip("Increases acceleration while at the apex of the jump.")]
	public float jumpHangAccelerationMult;
	[Tooltip("Increases maxSpeed while at the apex of the jump.")]
	public float jumpHangMaxSpeedMult;

	[Space(20)]
    #endregion

    #region JUMP ASSISTS
    [Header("Jump Assists")]
	[Tooltip("Grace period after falling off a platform, where you can still jump.")]
	[Range(0.01f, 0.5f)] public float coyoteTime;
	[Tooltip("//Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.")]
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; 

	[Space(20)]
    #endregion

    #region DASH
    [Header("Dash")]
	[Tooltip("Number of dashes the player can perform in a period of time.")]
	public int dashAmount;
	[Tooltip("The actual dash force applied.")]
	public float dashSpeed;
	[Tooltip("Duration for which the game freezes when we press dash but before we read directional input and apply a force.")]
	public float dashSleepTime;
	[Space(5)]
	
	[Tooltip("We keep the player's velocity at the dash speed during the drag phase.")]
	public float dashDragTime;
	[Space(5)]
	[Tooltip("Time after you finish the inital drag phase, smoothing the transition back to standard.")]
	public float dashEndTime;
	[Tooltip("Slows down player after a dash is performed.")]
	public Vector2 dashEndSpeed;
	[Tooltip("Reduces the affect of the player's movement while dashing.")]
	[Range(0f, 1f)] public float dashEndRunLerp;
	[Space(5)]
	[Tooltip("Time necessary to refill a dash after it's performed")]
	public float dashRefillTime;
	[Space(5)]
	[Tooltip("Time necessary to press the dash button again.")]
	[Range(0.01f, 0.5f)] public float dashInputBufferTime;
    #endregion

    //Unity Callback, called when the inspector updates
    private void OnValidate()
	{
		//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

		//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
		gravityScale = gravityStrength / Physics2D.gravity.y;

		//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

		//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

		#region Variable Ranges
		runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
		runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
		#endregion
	}

}
