public interface IDestructible
{
	bool Alive { get; }
	void TakeDamage(DamageData damageData);
}