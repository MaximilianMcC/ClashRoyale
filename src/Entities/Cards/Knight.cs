using System.Numerics;
using Raylib_cs;

class Knight : Troop
{
	public Knight(Team team) : base(team) { }

	public override int MaxHealth => 960;
	public override int Damage => 80;

	private static Texture2D sprite;

	// TODO: Don't make static
	public static void Load()
	{
		sprite = AssetManager.LoadTexture("./assets/knight.png");
	}

	// TODO: Don't make static
	public static void Unload()
	{
		Raylib.UnloadTexture(sprite);
	}

	public override void Render()
	{
		Raylib.DrawTexturePro(
			sprite,
			new Rectangle(0, 0, sprite.Dimensions),
			Hitbox,
			Vector2.Zero,
			0f,
			Color.White
		);
	}
}