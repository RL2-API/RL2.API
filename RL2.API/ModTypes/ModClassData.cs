namespace RL2.ModLoader;

public abstract class ModClassData : IRegistrable
{
	internal ClassData ClassData = new();
	/// <summary>
	/// Passive data of the created class
	/// </summary>
	public ClassPassiveData PassiveData => ClassData.m_passiveData;
	/// <summary>
	/// Spell data of the created class
	/// </summary>
	public ClassSpellData SpellData => ClassData.m_spellData;
	/// <summary>
	/// Talent data of the created class
	/// </summary>
	public ClassTalentData TalentData => ClassData.m_talentData;
	/// <summary>
	/// Weapon data of the created class
	/// </summary>
	public ClassWeaponData WeaponData => ClassData.m_weaponData;
	/// <summary>
	/// Stats data of the created class
	/// </summary>
	public ClassStatsData StatsData => ClassData.m_statsData;

	/// <summary>
	/// Used to set the classes data.
	/// </summary>
	public virtual void SetData() { }

	/// <summary>
	/// Registers the class
	/// </summary>
	public virtual void Register() {
		ClassLibrary.Instance.m_classLibrary.Add((ClassType)RL2API.LastUsedClassType++, ClassData);
	}
}