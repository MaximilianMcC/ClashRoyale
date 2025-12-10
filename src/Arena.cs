using System.Numerics;
using Raylib_cs;

static class Arena
{
	public static List<Deployable> Cards = [];

	public static async Task Update()
	{
		// Check for if new cards are spawned
		await CheckForNewCards();

		// Update the cards
		for (int i = 0; i < Cards.Count; i++)
		{
			Cards[i].Update();
		}
	}

	public static void Render()
	{
		// Render the cards
		for (int i = 0; i < Cards.Count; i++)
		{
			Cards[i].Render();
		}
	}

	private static async Task CheckForNewCards()
	{
		// Check for if we wanna spawn something
		{
			if (Raylib.IsMouseButtonPressed(MouseButton.Left))
			{
				Knight knight = new Knight(Team.Blue);
				knight.Spawn(Raylib.GetMousePosition());

				await Networker.SendMessage($"KNIGHT|{Team.Blue}|{Raylib.GetMousePosition()}");
			}

			if (Raylib.IsMouseButtonPressed(MouseButton.Right))
			{
				Knight knight = new Knight(Team.Red);
				knight.Spawn(Raylib.GetMousePosition());

				await Networker.SendMessage($"KNIGHT|{Team.Red}|{Raylib.GetMousePosition()}");
			}
		}

		// Check for if the other player is spawning something
		{
			string incomingMessage = await Networker.GetLastMessage();
			if (incomingMessage.StartsWith("KNIGHT"))
			{
				// Extract the properties
				Team team = ParseTeam(incomingMessage.Split("|")[1]);
				Vector2 spawnPoint = ParseVector(incomingMessage.Split("|")[2]);
				Networker.FlushLastMessage();

				// Spawn it
				Knight knight = new Knight(team);
				knight.Spawn(spawnPoint);
			}			
		}
	}

	// TODO: Put somewhere else
	private static Vector2 ParseVector(string vectorString)
	{
		string[] components = vectorString.Replace("<", "").Replace(">", "").Split(", ");
		return new Vector2(
			float.Parse(components[0]),
			float.Parse(components[1])
		);
	}

	private static Team ParseTeam(string teamString)
	{
		return (Team)Enum.Parse(typeof(Team), teamString);
	}
}