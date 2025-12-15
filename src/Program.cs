using System.Net.Sockets;
using System.Numerics;
using Raylib_cs;

class Program
{
	public static async Task Main(string[] args)
	{
		// Set up networking



		// Set up raylib
		Raylib.SetTraceLogLevel(TraceLogLevel.Warning);
		Raylib.InitWindow(540, 960, "clash ro" + (Networker.Hosting ? " (host)" : ""));
		Raylib.InitAudioDevice();

		Knight.Load();
		Skeleton.Load();

		KnightCard knight = new KnightCard();
		SkeletonCard skeleton = new SkeletonCard();

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

			knight.Update();
			// skeleton.Update();

			Raylib.BeginDrawing();
			Raylib.ClearBackground(Color.Green);
			Raylib.DrawText($"{Raylib.GetFPS()}", 10, 10, 30, Color.White);
			Arena.Render();
			laughingKing.Render();
			knight.Render();
			// skeleton.Render();
			Raylib.EndDrawing();
		}

		Knight.Unload();
		Skeleton.Unload();

		knight.CleanUp();
		skeleton.CleanUp();

		AssetManager.UnloadEverything();
		Raylib.CloseAudioDevice();
		Raylib.CloseWindow();
	}
}