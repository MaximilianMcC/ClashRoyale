using System.Numerics;
using Raylib_cs;

class Card
{
	private Texture2D texture;
	private Rectangle hitbox;

	private bool justDropped;
	private bool previouslyDragging;

	public Card(Texture2D cardTexture)
	{
		texture = cardTexture;
		hitbox = new Rectangle(0, 0, 100, 130);
	}

	public void Update()
	{
		Drag();

		if (justDropped)
		{
			// Spawn whatever we're spawning
			Spawn();

			// 'Remove' the card
			hitbox.Position = Vector2.Zero;
			previouslyDragging = false;
		}
	}

	// TODO: Rewrite
	private void Drag()
	{
		justDropped = false;
		bool draggingRn = Raylib.IsMouseButtonDown(MouseButton.Left) && Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), hitbox);

		// If we're dragging then drag the thing yk
		if (draggingRn)
		{
			hitbox.Position += Raylib.GetMouseDelta();
		}

		// Check for if we let go
		if (previouslyDragging == true && Raylib.IsMouseButtonReleased(MouseButton.Left))
		{
			justDropped = true;
		}

		previouslyDragging = draggingRn;
	}

	public void Render()
	{
		Raylib.DrawTexturePro(
			texture,
			new Rectangle(0, 0, texture.Dimensions),
			hitbox,
			Vector2.Zero,
			0f,
			Color.White
		);
	}

	public async virtual void Spawn()
	{
		Knight knight = new Knight(Team.Blue);
		knight.Spawn(hitbox.Position);

		await Networker.SendMessage($"KNIGHT|{Team.Blue}|{Raylib.GetMousePosition()}");
	}
}