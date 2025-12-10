using System.Net;
using System.Net.Sockets;
using System.Text;
using Raylib_cs;

class Networker
{
	public static bool Hosting { get; private set; }
	private static TcpListener server;
	private static TcpClient client;
	private static TcpClient otherClient;
	private static List<string> unreadMessages = [];

	public static bool UnreadMessages => unreadMessages.Count > 0;
	public static TcpClient Recipient => Hosting ? otherClient : client;

	public static async Task Network(string[] args)
	{
		// Check for if we have enough args
		if (args.Length != 3)
		{
			Console.WriteLine("probably incorrect args idk.\nClashRoyale.exe <IP> <PORT> <H|J>");
			return;
		}

		// Get the ip, port, and wether we're hosting/joining
		string ip = args[0];
		int port = int.Parse(args[1]);
		Hosting = args[2] == "H";		

		// if we're the host then make a game, and if we're
		// the client then join the existing game
		if (Hosting) await CreateGame(port);
		else await JoinGame(ip, port);

		// Wait for both clients and the server to be ready
		Console.WriteLine("Waiting for everyone to ready up");
		while (Recipient == null || Recipient.Connected == false) await Task.Delay(50);
	}

	private static async Task CreateGame(int port)
	{
		// Listen to any incoming data
		server = new TcpListener(IPAddress.Any, port);
		server.Start();

		// Begin listening in a separate background thread
		Console.WriteLine("Server listening on port " + port);
		_ = Task.Run(async () =>
		{
			// Get a client
			otherClient = await server.AcceptTcpClientAsync();
			_ = HandleClient(otherClient);
		});
	}

	private static async Task JoinGame(string ip, int port)
	{
		// Join the server
		client = new TcpClient();
		await client.ConnectAsync(ip, port);
		Console.WriteLine("Joined server");

		// Let ourself listen to incoming data from the server
		_ = HandleClient(client);
	}

	private static async Task HandleClient(TcpClient remoteClient)
	{
		// Get the stream of incoming data
		NetworkStream stream = remoteClient.GetStream();
		byte[] buffer = new byte[1024];
		int bytesRead;

		try
		{
			// Listen to everything sent
			while ((bytesRead = await stream.ReadAsync(buffer)) != 0)
			{
				// Decode the data from the stream
				string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

				// Store the data so we can give it when queried
				unreadMessages.Add(data);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("client probably left idk (please close the window)\n" + e.Message);
		}
		finally
		{
			// Disconnect the client
			remoteClient.Close();
		}
	}

	public static async Task SendMessage(string message)
	{
		// If we're hosting then send data to the client,
		// and if we're joining then send data to the server
		TcpClient recipient = Hosting ? otherClient : client;

		// Check for if the're available
		if (recipient != null && recipient.Connected)
		{
			// Encode the string to bytes
			byte[] data = Encoding.UTF8.GetBytes(message);
			
			// Send it
			NetworkStream stream = recipient.GetStream();
			await stream.WriteAsync(data);
		}
	}

	// Get all messages
	public static async Task<List<string>> GetMessages()
	{
		List<string> messages = unreadMessages;
		unreadMessages.Clear();

		return messages;
	}

	// Get just the least-recent/oldest message
	public static async Task<string> GetMessage()
	{
		string message = unreadMessages.FirstOrDefault();
		unreadMessages.Remove(message);

		return message;
	}
}