using Raylib_cs;

class Program
{
	public static async Task Main(string[] args)
	{
		// Set up networking
		await Networker.Network(args);

		// Set up raylib
		Raylib.SetTraceLogLevel(TraceLogLevel.Warning);
		Raylib.InitWindow(540, 960, "clash ro" + (Networker.Hosting ? " (host)" : ""));

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