using System.Numerics;
using Raylib_cs;

abstract class Deployable
{
	public abstract string Name { get; }
	public abstract int MaxHealth { get; }
	public int Health { get; set; }
	public Team Team { get; }
	public virtual float Speed { get; set; }
	public Rectangle Hitbox;
	
	public Color TeamColor => Team == Team.Red ? Color.Red : Color.Blue;

	public Vector2 Position
	{
		get => Hitbox.Position;
		set => Hitbox.Position = value;
	}

	public Deployable(Team team)
	{
		Team = team;
		Health = MaxHealth;

		// Set the default hitbox size
		Hitbox.Size = new Vector2(40, 60);
	}

	public virtual async Task Spawn(Vector2 spawnPoint)
	{
		// Set the position
		Position = spawnPoint;

		// Add it to the map
		Arena.Cards.Add(this);

		// Network it
		await Networker.SendMessage($"{Name}|{Team}|{Position}");
		Console.WriteLine("sent packet");
	}

	public virtual void Update() { }
	public virtual void Render() { }
	public virtual void CleanUp() { }

	protected void RenderHitbox()
	{
		Raylib.DrawRectangleLinesEx(Hitbox, 2f, Color.Magenta);
	}

	// TODO: optional option to optionally draw the health number on it
	protected void RenderHealthBar()
	{
		float width = Hitbox.Width;
		float filledUpBar = (Health / MaxHealth) * width;
		Rectangle bar = new Rectangle(Position, width, 10f);

		// Draw the background and content
		Raylib.DrawRectangleRec(bar, Color.Gray);
		bar.Width = filledUpBar;
		Raylib.DrawRectangleRec(bar, TeamColor);
	}

	// TODO: Greedy path finding
	public void MoveTowards(Deployable target)
	{
		if (target == null) return;

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
		direction = Vector2.Normalize(direction);
		Vector2 newPosition = Position + (direction * Speed) * Raylib.GetFrameTime();

		// If there's no collision then move. Otherwise don't
		if (Collision(newPosition) == false) Position = newPosition;
	}

	// TODO: 'refine'
	private bool Collision(Vector2 newPosition)
	{
		// Adjust ourself
		Rectangle newHitbox = Hitbox;
		newHitbox.Position = newPosition;

		// Loop over everything on the arena
		foreach (Deployable card in Arena.Cards)
		{
			if (card == this) continue;
			if (Raylib.CheckCollisionRecs(card.Hitbox, newHitbox)) return true;
		}

		return false;
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