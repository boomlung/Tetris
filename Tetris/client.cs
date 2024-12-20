using System;
using System.Net.Sockets;
using System.Text;

class Client
{
    public static void Main()
    {
        // Define the server address and port number to connect to
        string server = "127.0.0.1";
        int port = 8080;

        // Create a TCP client
        TcpClient client = new TcpClient(server, port);
        Console.WriteLine("Connected to the server.");

        // Get a stream object for reading and writing
        NetworkStream stream = client.GetStream();

        // Send a message to the server
        byte[] message = Encoding.ASCII.GetBytes("Hello from the client");
        stream.Write(message, 0, message.Length);
        Console.WriteLine("Message sent.");

        // Read the response from the server
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Received: {0}", response);

        // Close the connection
        client.Close();
    }
}
