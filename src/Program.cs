using Raylib_cs;

class Program
{
	public static async Task Main(string[] args)
	{
		// Set up networking
		await Networker.Network(args);

		Raylib.SetTraceLogLevel(TraceLogLevel.Warning);
		Raylib.InitWindow(540, 960, "clash ro" + (Networker.Hosting ? " (host)" : ""));

		AssetManager.Textures["debug"] = AssetManager.LoadTexture("./assets/debug.png");

		while (Raylib.WindowShouldClose() == false)
		{
			if (Raylib.IsKeyPressed(KeyboardKey.Space)) _ = Networker.SendData("hi from " + (Networker.Hosting ? "host" : "client"));

			Raylib.BeginDrawing();
			Raylib.ClearBackground(Networker.Hosting ? Color.Green : Color.Blue);
			Raylib.EndDrawing();
		}

		AssetManager.UnloadEverything();
		Raylib.EndDrawing();
	}
}