using System.Numerics;
using Raylib_cs;

abstract class Deployable
{
	public Team Team { get; }
	public Rectangle Hitbox;
	
	public Vector2 Position
	{
		get => Hitbox.Position;
		set => Hitbox.Position = value;
	}

	public Deployable(Team team)
	{
		// Set the team
		Team = team;
	}

	public virtual void Spawn(Vector2 position)
	{
		// Set the position
		Position = position;

		// Add it to the map
		Arena.Cards.Add(this);

		Console.WriteLine($"spawned a {Team} one");
	}

	public virtual void Update() { }
	public virtual void Render() { }
	public virtual void CleanUp() { }
}

enum Team
{
	Red,
	Blue
}