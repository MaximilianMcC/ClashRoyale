using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

public static class Networker
{
	public static bool Host { get; private set; }
	public static readonly ConcurrentQueue<string> IncomingPackets = [];
	private static readonly ConcurrentQueue<string> outgoingPackets = [];

	public static async Task Network(string ip, int port, bool host)
	{
		// If we are the host, then start a server
		Host = host;
		if (Host) await Server.Init(port);

		// Join the server
		await Client.ConnectToServerAsync(ip, port);
	}

	public static class Server
	{
		private static TcpListener server;
		private static List<TcpClient> clients = [];

		public static async Task Init(int port)
		{
			// Start the server to listen to all incoming traffic
			server = new TcpListener(IPAddress.Any, port);
			server.Start();

			while (true)
			{
				// Listen for new clients
				TcpClient client = await server.AcceptTcpClientAsync();
				clients.Add(client);

				// Handle packets
				_ = HandleIncomingPackets(client);
				_ = HandleOutgoingPackets(client);
			}
		}

		private static async Task HandleIncomingPackets(TcpClient client)
		{
			// Get the current clients stream
			NetworkStream stream = client.GetStream();

			// Read the message to the buffer
			byte[] buffer = new byte[1024];

			while (true)
			{
				// Collect incoming data if we've been sent anything
				int bytes = await stream.ReadAsync(buffer);
				if (bytes == 0) break;

				// Decode the bytes
				string message = Encoding.UTF8.GetString(buffer, 0, bytes);
				IncomingPackets.Enqueue(message);
			}
		}

		private static async Task HandleOutgoingPackets(TcpClient client)
		{
			// Get the current clients stream
			NetworkStream stream = client.GetStream();

			while (true)
			{
				// Check for if any data is to be sent
				if (outgoingPackets.TryDequeue(out string message))
				{
					// Check for what needs to happen to this packet
					// Then afterwards remove the header
					char headerType = message[0];
					message = message.Split("§")[1];



					// Serialise then send the packet
					byte[] packet = Encoding.UTF8.GetBytes(message + "\n");
					await stream.WriteAsync(packet);
				}

				// Take a little break to stop cpu burn idk
				await Task.Delay(1);
			}
		}

		public static void SendPacketTo(TcpClient client, string message)
		{
			// Encode the message for sending
			byte[] data = Encoding.UTF8.GetBytes(message);

			_ = client.GetStream().WriteAsync(data);
		}

		public static void SendPacketToAllClients(string message)
		{
			// Encode the message for sending
			byte[] data = Encoding.UTF8.GetBytes(message);

			// Loop over all clients
			foreach (TcpClient client in clients)
			{
				
			}
		}
	}

	public static class Client
	{
		private static TcpClient client;
		private static NetworkStream stream;

		public static async Task ConnectToServerAsync(string ip, int port)
		{
			// Create the client then connect to the server
			client = new TcpClient();
			await client.ConnectAsync(ip, port);

			// Set up the stream we'll between the server
			stream = client.GetStream();

			// Send and receive packets between the server
			_ = Task.Run(HandleIncomingPackets);
			_ = Task.Run(HandleOutgoingPackets);
		}

		// Sending stuff
		private static async Task HandleOutgoingPackets()
		{
			while (true)
			{
				// Check for if any data is to be sent
				if (outgoingPackets.TryDequeue(out string message))
				{
					// Serialise then send the packet
					byte[] packet = Encoding.UTF8.GetBytes(message + "\n");
					await stream.WriteAsync(packet);
				}

				// Take a little break to stop cpu burn idk
				await Task.Delay(1);
			}
		}

		// Receiving stuff
		private static async Task HandleIncomingPackets()
		{
			// Read the message to the buffer
			byte[] buffer = new byte[1024];

			while (true)
			{
				// Collect incoming data if we've been sent anything
				int bytes = await stream.ReadAsync(buffer);
				if (bytes == 0) break;

				// Decode the bytes
				string message = Encoding.UTF8.GetString(buffer, 0, bytes);
				IncomingPackets.Enqueue(message);
			}
		}

		public static void SendPacketToServer(string message)
		{
			// Add the server header then send the message
			message = "s§" + message;
			outgoingPackets.Enqueue(message);
		}

		public static void SendPacketToClient(TcpClient client, string message)
		{
			// Add the client header then send the message
			// TODO: Parse/handle the packet
			message = $"c<{client}>§" + message;
			outgoingPackets.Enqueue(message);
		}

		public static void SendPacketToAllClients(string message)
		{
			// Add the all clients header then send the message
			message = "a§" + message;
			outgoingPackets.Enqueue(message);
		}

		public static void SendPacketToAllClientsExceptMyself(string message)
		{
			// Add the all but me header then send the message
			message = "x§" + message;
			outgoingPackets.Enqueue(message);	
		}
	}
}