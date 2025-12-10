using System.Numerics;
using Raylib_cs;

abstract class Deployable
{
	public abstract int MaxHealth { get; }
	public int Health { get; set; }
	public Team Team { get; }
	public virtual float Speed { get; set; }
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

		// Set the default hitbox size
		Hitbox.Size = new Vector2(40, 60);
	}

	public virtual void Spawn(Vector2 position)
	{
		// Set the position
		Position = position;

		// Add it to the map
		Arena.Cards.Add(this);
	}

	public virtual void Update() { }
	public virtual void Render() { }
	public virtual void CleanUp() { }

	protected void RenderHitbox()
	{
		Raylib.DrawRectangleLinesEx(Hitbox, 2f, Color.Magenta);
	}

	// TODO: Greedy path finding
	public void MoveTowards(Deployable target)
	{
		// Get the direction we need to move in
		// TODO: Clamp it to like angles of 45deg or something
		Vector2 direction = target.Position - Position;

		// If we're pretty much there then stop otherwise
		// we'll get a floating point error and go in some
		// random as direction (not good (gone walkabouts))
		if (direction.LengthSquared() <= 0.1f)
		{
			Position = target.Position;
			return;
		}

		// Move in that direction
		// TODO: Collision
		direction = Vector2.Normalize(direction);
		Position += (direction * Speed) * Raylib.GetFrameTime();
	}

	public Deployable GetClosestEnemy()
	{
		return Arena.Cards
			.Where(card => card.Team != Team)
			.OrderBy(card => Vector2.Distance(card.Position, Position))
			.FirstOrDefault();
	}
}

enum Team
{
	Red,
	Blue
}