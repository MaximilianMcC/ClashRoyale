using System.Numerics;
using Raylib_cs;

class Knight : InfantryTroop
{
	public Knight(Team team) : base(team) { }

	public override string Name => "Knight";
	public override int MaxHealth => 960;
	public override int Damage => 80;
	public override float Speed => 40f;
	
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

	public override void Update()
	{
		// Move towards the closest enemy
		MoveTowards(GetClosestEnemy());
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
		RenderHitbox();
		RenderHealthBar();
	}
}

class KnightCard : Card
{
	public KnightCard() : base(Raylib.LoadTexture("./assets/card-knight.png")) { }

	public override async Task Spawn()
	{
		Knight knight = new Knight(Team.Blue);
		await knight.Spawn(CardPosition);
	}
}