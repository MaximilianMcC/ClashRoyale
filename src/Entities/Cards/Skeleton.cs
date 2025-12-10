using System.Numerics;
using Raylib_cs;

class Skeleton : InfantryTroop
{
	public Skeleton(Team team) : base(team) { }

	public override int MaxHealth => 20;
	public override int Damage => 30;
	public override float Speed => 60f;
	
	private static Texture2D sprite;

	// TODO: Don't make static
	public static void Load()
	{
		sprite = AssetManager.LoadTexture("./assets/skeleton.png");
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