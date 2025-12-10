class Knight : Troop
{
	public Knight(Team team) : base(team) { }

	public override int MaxHealth => 960;
	public override int Damage => 80;
}