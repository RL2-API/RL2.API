namespace RL2.ModLoader;

public class StatBonuses
{
    public int armor = 0;

    public float critDamage = 0.0f;

    public float dexterity = 0.0f;

    public float magicCritDamage = 0.0f;

    public float focus = 0.0f;

    public float resolve = 0.0f;

    public int vitality = 0;

    public float strength = 0.0f;

    public float inteligence = 0.0f;

    public float maxHealth = 0.0f;

    public int maxMana = 0;

    /// <summary>
    /// Added to character move speed before Haste runes. <br></br>
    /// Base movement speed is 12.
    /// </summary>
    public float movementSpeed = 0.0f;

    /// <summary>
    /// Base jump height is 8.75.
    /// </summary>
    public float jumpHeight = 0.0f;

    /// <summary>
    /// Base souble jump height is 4.75.
    /// </summary>
    public float doubleJumpHeight = 0.0f;

    public int extraJumps = 0;

    /// <summary>
    /// Base dash distance is 8.
    /// </summary>
    public float dashDistance = 0f;

    /// <summary>
    /// Base dash force is 26.
    /// </summary>
    public float dashForce = 0f;

    /// <summary>
    /// Used to increase dash cooldown. <br></br>
    /// Base cooldown is 0, so DO NOT make this negative.
    /// </summary>
    public float dashCooldown = 0f;

    public int extraDashes = 0;

    public void ResetAll()
    {
        armor = 0;
        critDamage = 0.0f;
        dexterity = 0.0f;
        magicCritDamage = 0.0f;
        focus = 0.0f;
        resolve = 0.0f;
        vitality = 0;
        strength = 0.0f;
        inteligence = 0.0f;
        maxHealth = 0.0f;
        maxMana = 0;
        movementSpeed = 0.0f;
        jumpHeight = 0.0f;
        doubleJumpHeight = 0.0f;
        extraJumps = 0;
        dashDistance = 0f;
        dashForce = 0f;
        dashCooldown = 0f;
        extraDashes = 0;
    }
}
