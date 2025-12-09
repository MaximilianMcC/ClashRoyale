using Raylib_cs;

class Program
{
	public static void Main(string[] args)
	{
		Raylib.SetTraceLogLevel(TraceLogLevel.Warning);
		Raylib.InitWindow(540, 960, "clash ro");

		AssetManager.Textures["debug"] = AssetManager.LoadTexture("./assets/debug.png");

		while (Raylib.WindowShouldClose() == false)
		{
			Raylib.BeginDrawing();
			Raylib.ClearBackground(Color.Green);
			Raylib.EndDrawing();
		}

		AssetManager.UnloadEverything();
		Raylib.EndDrawing();
	}
}