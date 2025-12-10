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
		Raylib.InitAudioDevice();

		Emote laughingKing = new Emote("./assets/emotes/laughing-king.png", "./assets/emotes/laughing-king.wav", 355);

		AssetManager.Textures["debug"] = AssetManager.LoadTexture("./assets/debug.png");

		while (Raylib.WindowShouldClose() == false)
		{
			Arena.Update();

			if (await Networker.GetMessage() == "EMOTE")
			{
				laughingKing.Play();
			}

			if (Raylib.IsKeyPressed(KeyboardKey.Space))
			{
				laughingKing.Play();
				await Networker.SendMessage("EMOTE");
			}

			Raylib.BeginDrawing();
			Raylib.ClearBackground(Color.Green);
			Arena.Render();
			laughingKing.Render();
			Raylib.EndDrawing();
		}

		AssetManager.UnloadEverything();
		Raylib.CloseAudioDevice();
		Raylib.CloseWindow();
	}
}