abstract class Troop : Deployable
{
	public Troop(Team team) : base(team) { }

	public abstract int MaxHealth { get; }
	public int Health { get; set; }

	public abstract int Damage { get; }
}