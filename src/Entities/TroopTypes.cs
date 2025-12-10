abstract class InfantryTroop : Deployable
{
	public InfantryTroop(Team team) : base(team) { }

	public abstract int Damage { get; }
}

abstract class AerialTroop : Deployable
{
	public AerialTroop(Team team) : base(team) { }

	public abstract int Damage { get; }
}