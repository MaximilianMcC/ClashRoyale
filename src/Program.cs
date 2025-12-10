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
		Skeleton.Load();

		Texture2D texture = AssetManager.LoadTexture("./assets/card-knight.png");
		Card card = new Card(texture);

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

			card.Update();

			Raylib.BeginDrawing();
			Raylib.ClearBackground(Color.Green);
			Raylib.DrawText($"{Raylib.GetFPS()}", 10, 10, 30, Color.White);
			Arena.Render();
			laughingKing.Render();
			card.Render();
			Raylib.EndDrawing();
		}

		Knight.Unload();
		Skeleton.Unload();

		Raylib.UnloadTexture(texture);

		AssetManager.UnloadEverything();
		Raylib.CloseAudioDevice();
		Raylib.CloseWindow();
	}
}