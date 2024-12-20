using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    public void server()
    {
        // Define the IP address and port number to listen on
        IPAddress ip = IPAddress.Any;
        int port = 8080;

        // Create a TCP listener
        TcpListener listener = new TcpListener(ip, port);
        listener.Start();
        Console.WriteLine("Server started. Waiting for a connection...");

        // Accept incoming connection
        TcpClient client = listener.AcceptTcpClient();
        Console.WriteLine("Client connected.");

        // Get a stream object for reading and writing
        NetworkStream stream = client.GetStream();

        // Read data sent by the client
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Received: {0}", message);

        // Send a response back to the client
        byte[] response = Encoding.ASCII.GetBytes("Hello from the server");
        stream.Write(response, 0, response.Length);
        Console.WriteLine("Response sent.");

        // Close the connection
        client.Close();
        listener.Stop();
    }
}
