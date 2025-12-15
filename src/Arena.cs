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
		// Check for if the other player is spawning something
		string incomingMessage = await Networker.GetLastMessage();
		if (incomingMessage.StartsWith("Knight"))
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