namespace RL2.ModLoader;

public sealed class PlayerModAbilityBonuses
{
	#region Flat bonuses
	public int numberOfJumps = 0;
	public float moveSpeed = 0;
	public float accelerationOnGround = 0;
	public float accelerationInAir = 0;

	public float jumpHeight = 0;
	public float doubleJumpHeight = 0;
	public float jumpReleaseForce = 0;

	public int numberOfDashes = 0;
	public bool enableOmnidash = false;
	public float dashDistance = 0;
	public float dashForce = 0;
	
	public bool resetsDoubleJump = false;
	public bool resetsDash = false;
	public float spinKickAttackSpeed = 0;
	public float spinKickBounceHeight = 0;
	#endregion

	#region Multipliers
	public float moveSpeedMultiplier = 1f;
	public float accelerationOnGroundMultiplier = 1f;
	public float accelerationInAirMultiplier = 1f;

	public int numberOfJumpsMultiplier = 1;
	public float jumpHeightMultiplier = 1f;
	public float doubleJumpHeightMultiplier = 1f;
	public float jumpReleaseForceMultiplier = 1f;

	public int numberOfDashesMultiplier = 1;
	public float dashDistanceMultiplier = 1f;
	public float dashForceMultiplier = 1f;

	public float spinKickAttackSpeedMultiplier = 1f;
	public float spinKickBounceHeightMultiplier = 1f;
	#endregion

	public void Reset() { 
		//TODO
	}
}