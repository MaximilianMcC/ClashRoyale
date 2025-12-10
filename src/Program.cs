using System.Numerics;
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

		Knight.Load();

		Knight knight = new Knight(Team.Blue);
		knight.Spawn(new Vector2(0, 0));
		Knight knight2 = new Knight(Team.Red);
		knight2.Spawn(new Vector2(100, 100));

		Emote laughingKing = new Emote("./assets/emotes/laughing-king.png", "./assets/emotes/laughing-king.wav", 355);

		AssetManager.Textures["debug"] = AssetManager.LoadTexture("./assets/debug.png");

		while (Raylib.WindowShouldClose() == false)
		{
			await Arena.Update();

			if (await Networker.GetLastMessage() == "EMOTE")
			{
				laughingKing.Play();
				Networker.FlushLastMessage();
			}

			if (Raylib.IsKeyPressed(KeyboardKey.Space))
			{
				await Networker.SendMessage("EMOTE");
				laughingKing.Play();
			}

			Raylib.BeginDrawing();
			Raylib.ClearBackground(Color.Green);
			Arena.Render();
			laughingKing.Render();
			Raylib.EndDrawing();
		}

		Knight.Unload();
		AssetManager.UnloadEverything();
		Raylib.CloseAudioDevice();
		Raylib.CloseWindow();
	}
}